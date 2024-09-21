using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


namespace DoozyPractice.Gameplay
{
    public enum OverCategory : int
    {
        One = 1,
        Two = 2,
        Four = 4,
        Six = 6,
        AllOut = -1     // To denote that there is no over limit
    }

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

        [SerializeField]
        int _totalWickets;

        [SerializeField, Tooltip("Duration for each turn in seconds")]
        int _turnDuration;

        [SerializeField, Tooltip("Wait for seconds, before starting next turn")]
        int _waitBeforeNextTurn;

        public bool IsOwnerBatting { get; private set; }
        public int OwnerTotalScore { get; private set; }
        public int OtherTotalScore { get; private set; }


        int _currentBallCount = 0;
        int _currentOverCount = 0;
        int _ownerInputScore = 0;
        int _otherInputScore = 0;
        int _turnScore = 0;
        int _ownerOutCount = 0;
        int _otherOutCount = 0;

        float _timeElapsedSinceTurnCountdownStarted = 0;

        bool _isOutOnThisTurn = false;
        bool _pauseCountdown = true;

        void Start()
        {
            ResetUIs();
        }


        void Update()
        {
            TryExecuteNextTurn();
        }

        public void SetTotalOvers(OverCategory overCategory)
        {
            _totalOvers = (int)overCategory;     // 

            _gameplayUIMediator.UpdateOwnerTotalOversUI(0, 0, _totalOvers);
            _gameplayUIMediator.UpdateOtherTotalOversUI(0, 0, _totalOvers);
        }

        public void SetTotalWickets(int count)
        {
            _totalWickets = count;

            _gameplayUIMediator.UpdateOwnerTotalWicketsUI(0, _totalWickets);
            _gameplayUIMediator.UpdateOtherTotalWicketsUI(0, _totalWickets);
        }

        public void ChangeBatsman(bool isOwnerBatting)
        {
            IsOwnerBatting = isOwnerBatting;

            if (IsOwnerBatting)
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

            CalculateScoreAndWicketsLost();
            UpdateScoreboardUIs();

            await Task.Delay(_waitBeforeNextTurn * 1000);

            if (TargetScoreReached() || HasLastTeamTotalOversEnded() || IsLastTeamAllOut())
            {
                EndGame();
                return;
            }

            if (IsOverComplete())
            {
                _currentOverCount++;
                ResetOver();
            }

            if (HasTotalOversEnded() || IsAllOut())
            {
                ExecuteHalfTIme();
                return;
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
            _isOutOnThisTurn = false;
            OnNextTurnCountdownStarted?.Invoke();
        }

        void EndGame()
        {
            _gameStateManager.ChangeGameState(GameStateCategory.GameEnd);
        }

        void CalculateScoreAndWicketsLost()
        {
            // Check if is out
            _isOutOnThisTurn = _ownerInputScore == _otherInputScore;

            _turnScore = _isOutOnThisTurn ? 0 : IsOwnerBatting ? _ownerInputScore : _otherInputScore;

            // Calculate score
            if (IsOwnerBatting)
                OwnerTotalScore += _turnScore;
            else
                OtherTotalScore += _turnScore;

            if (!_isOutOnThisTurn)
                return;

            // Calculate wicket
            if (IsOwnerBatting)
                _ownerOutCount++;
            else
                _otherOutCount++;
        }

        void UpdateScoreboardUIs()
        {
            _gameplayUIMediator.UpdateOwnerInputScoreUI(_ownerInputScore);
            _gameplayUIMediator.UpdateOtherInputScoreUI(_otherInputScore);

            if (IsOwnerBatting)
            {
                _gameplayUIMediator.UpdateOwnerTotalScoreUI(OwnerTotalScore);
                _gameplayUIMediator.UpdateOwnerTotalWicketsUI(_ownerOutCount, _totalWickets);
                _gameplayUIMediator.UpdateOwnerTotalOversUI(_currentOverCount, _currentBallCount, _totalOvers);
            }
            else
            {
                _gameplayUIMediator.UpdateOtherTotalScoreUI(OtherTotalScore);
                _gameplayUIMediator.UpdateOtherTotalWicketsUI(_otherOutCount, _totalWickets);
                _gameplayUIMediator.UpdateOtherTotalOversUI(_currentOverCount, _currentBallCount, _totalOvers);
            }

            _gameplayUIMediator.UpdateOverScoreUI(_currentBallCount, _turnScore, _isOutOnThisTurn);
        }

        bool IsOverComplete() => _currentBallCount == _ballsPerOver;

        bool HasTotalOversEnded() => HasLimitedOver() && IsThisLastOver();

        bool HasLastTeamTotalOversEnded() => HaveBothPlayersBatted() && HasTotalOversEnded();

        bool HasLimitedOver() => _totalOvers != (int)OverCategory.AllOut;

        bool IsThisLastOver() => _currentOverCount == _totalOvers;

        bool IsAllOut() => IsOwnerBatting ? _ownerOutCount == _totalWickets : _otherOutCount == _totalWickets;

        bool IsLastTeamAllOut() => HaveBothPlayersBatted() && IsAllOut();

        bool TargetScoreReached() => HaveBothPlayersBatted() && (HasOwnerReachedOtherTarget() || HasOtherReachedOwnerTarget());

        bool HasOwnerReachedOtherTarget() => IsOwnerBatting && OwnerTotalScore >= OtherTotalScore;

        bool HasOtherReachedOwnerTarget() => !IsOwnerBatting && OtherTotalScore >= OwnerTotalScore;

        bool HaveBothPlayersBatted() => _gameStateManager.OwnerDidBatting && _gameStateManager.OtherDidBatting;

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
            _gameplayUIMediator.UpdateOwnerTotalWicketsUI(0, 0);
            _gameplayUIMediator.UpdateOtherTotalWicketsUI(0, 0);
            _gameplayUIMediator.UpdateOwnerTotalOversUI(0, 0, 0);
            _gameplayUIMediator.UpdateOtherTotalOversUI(0, 0, 0);
        }
    }
}
