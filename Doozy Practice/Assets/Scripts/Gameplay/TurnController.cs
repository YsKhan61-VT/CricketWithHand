using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


namespace DoozyPractice.Gameplay
{
    public class TurnController : MonoBehaviour
    {
        /// <summary>
        /// Called when the next turn is starting. User input panel needs to be activated.
        /// </summary>
        public UnityEvent OnNextTurnCountdownStarted;

        /// <summary>
        /// Called when the next turn duration ends. User input panel needs to be deactivated.
        /// </summary>
        public UnityEvent OnNextTurnCountdownEnded;

        [SerializeField]
        GameStateManager _gameStateManager;

        [SerializeField]
        GameplayUIMediator _gameplayUIMediator;

        [SerializeField]
        int _ballsPerOver;

        [SerializeField]
        int _totalOvers;

        [SerializeField, Tooltip("Duration for each turn in seconds")]
        int _turnDuration;

        [SerializeField, Tooltip("Wait for seconds, before starting next turn")]
        int _waitBeforeNextTurn;

        public bool IsOwnerBatting { get; private set; }
        public int OwnerTotalScore { get; private set; }
        public int OtherTotalScore { get; private set; }


        private int _currentBallCount;
        private int _currentOverCount;
        private int _ownerInputScore;
        private int _otherInputScore;
        private int _turnScore;

        private float _timeElapsedSinceTurnCountdownStarted;

        private bool _isOutOnThisTurn;
        private bool _pauseCountdown = true;

        private void Start()
        {
            ResetUIs();
        }


        private void Update()
        {
            TryExecuteNextTurn();
        }

        public void SetTotalOvers(int totalOvers)
        {
            _totalOvers = totalOvers;
            _gameplayUIMediator.UpdateOwnerTotalOversUI(0, 0, _totalOvers);
            _gameplayUIMediator.UpdateOtherTotalOversUI(0, 0, _totalOvers);
        }

        public void ChangeBatsman(bool isOwnerBatting)
        {
            IsOwnerBatting = isOwnerBatting;

            if (isOwnerBatting)
            {
                _gameStateManager.ChangeGameState(GameStateCategory.Owner_Batting);
            }
            else
            {
                _gameStateManager.ChangeGameState(GameStateCategory.Other_Batting);
            }
        }

        public void StartGameplay()
        {
            if (!PassPreChecks()) return;

            ResetAtStart();
            StartNextTurn();
        }

        public void RegisterOwnerInput(int scoreValue) =>
            _ownerInputScore = scoreValue;

        public void RegisterOtherInput(int scoreValue) =>
            _otherInputScore = scoreValue;

        public void StartNextHalf()
        {
            ChangeBatsman(!IsOwnerBatting);
            _currentOverCount = 0;
            _gameplayUIMediator.ResetOverScoreUI();
            _currentBallCount = 0;
            StartNextTurn();
        }

        async void TryExecuteNextTurn()
        {
            if (!HasTurnCountdownEnded())
                return;

            OnNextTurnCountdownEnded?.Invoke();

            _currentBallCount++;

            CalculateScore();
            UpdateScoresAndOversUI();

            await Task.Delay(_waitBeforeNextTurn * 1000);

            if (IsOverComplete())
            {
                _currentOverCount++;

                if (IsBattingComplete())
                {
                    if (HaveBothPlayersBatted())
                    {
                        EndGame();
                        return;
                    }

                    ExecuteHalfTIme();
                    return;
                }

                ResetOver();
            }

            StartNextTurn();
        }

        void ResetOver()
        {
            _gameplayUIMediator.ResetOverScoreUI();
            _currentBallCount = 0;
        }

        void ExecuteHalfTIme()
        {
            _gameStateManager.ChangeGameState(GameStateCategory.HalfTime);
        }

        bool HasTurnCountdownEnded()
        {
            if (_pauseCountdown) return false;

            _timeElapsedSinceTurnCountdownStarted += Time.deltaTime;
            _gameplayUIMediator.UpdateTurnDurationSlider(1 - (_timeElapsedSinceTurnCountdownStarted / _turnDuration));

            if (_timeElapsedSinceTurnCountdownStarted < _turnDuration)
                return false;
            else
            {
                _pauseCountdown = true;
                return true;
            }
        }

        void StartNextTurn()
        {
            _timeElapsedSinceTurnCountdownStarted = 0;
            _gameplayUIMediator.UpdateTurnDurationSlider(1);
            _pauseCountdown = false;
            _turnScore = 0;
            _ownerInputScore = 0;
            _otherInputScore = 0;
            OnNextTurnCountdownStarted?.Invoke();
        }

        void EndGame()
        {
            _gameStateManager.ChangeGameState(GameStateCategory.GameEnd);
        }

        void CalculateScore()
        {
            _isOutOnThisTurn = (_ownerInputScore == _otherInputScore);

            _turnScore = (_ownerInputScore == _otherInputScore) ? 0 :
                IsOwnerBatting ? _ownerInputScore : _otherInputScore;

            if (IsOwnerBatting)
            {
                OwnerTotalScore += _turnScore;
            }
            else
            {
                OtherTotalScore += _turnScore;
            }
        }

        void UpdateScoresAndOversUI()
        {
            _gameplayUIMediator.UpdateOwnerInputScoreUI(_ownerInputScore);
            _gameplayUIMediator.UpdateOtherInputScoreUI(_otherInputScore);

            if (IsOwnerBatting)
            {
                _gameplayUIMediator.UpdateOwnerTotalScoreUI(OwnerTotalScore);
                _gameplayUIMediator.UpdateOwnerTotalOversUI(_currentOverCount, _currentBallCount, _totalOvers);
            }
            else
            {
                _gameplayUIMediator.UpdateOtherTotalScoreUI(OtherTotalScore);
                _gameplayUIMediator.UpdateOtherTotalOversUI(_currentOverCount, _currentBallCount, _totalOvers);
            }

            _gameplayUIMediator.UpdateOverScoreUI(_currentBallCount, _turnScore, _isOutOnThisTurn);
        }

        bool IsOverComplete() => _currentBallCount == _ballsPerOver;
        bool IsBattingComplete() => _currentOverCount == _totalOvers;
        bool HaveBothPlayersBatted() => _gameStateManager.OwnerDidBatting && _gameStateManager.OtherDidBatting;

        void ResetAtStart()
        {
            OwnerTotalScore = 0;
            OtherTotalScore = 0;
            _currentBallCount = 0;
            _currentOverCount = 0;
        }

        bool PassPreChecks()
        {
            if (_ballsPerOver == 0 || _totalOvers == 0)
            {
                gameObject.SetActive(false);
                return false;
            }
            return true;
        }

        void ResetUIs()
        {
            _gameplayUIMediator.ResetOverScoreUI();
            _gameplayUIMediator.UpdateOwnerTotalScoreUI(0);
            _gameplayUIMediator.UpdateOtherTotalScoreUI(0);
            _gameplayUIMediator.UpdateOwnerTotalOversUI(0, 0, 0);
            _gameplayUIMediator.UpdateOtherTotalOversUI(0, 0, 0);
        }
    }
}
