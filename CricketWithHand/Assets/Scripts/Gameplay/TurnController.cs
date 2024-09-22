using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


namespace CricketWithHand.Gameplay
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

            if (IsGameEndConditionsMatching())
            {
                EndGame();
                return;
            }

            if (IsCurrentOverComplete())
            {
                _currentOverCount++;
                ResetOver();
            }

            if (HasTotalOversOfThisHalfEnded() || IsCurrentHalfBattingTeamAllOut())
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

        /// <summary>
        /// Is the total balls of the current over completed playing.
        /// </summary>
        bool IsCurrentOverComplete() => _currentBallCount == _ballsPerOver;

        /// <summary>
        /// Does the game have limited overs to play or until all wickets are lost.
        /// </summary>
        bool HasLimitedOver() => _totalOvers != (int)OverCategory.AllOut;

        /// <summary>
        /// Is the current over the last over of this half(1st half or 2nd half)
        /// </summary>
        bool IsLastOverOfThisHalf() => _currentOverCount == _totalOvers;

        /// <summary>
        /// Has the current half batting team lost all wickets.
        /// </summary>
        bool IsCurrentHalfBattingTeamAllOut() => IsOwnerBatting ? _ownerOutCount == _totalWickets : _otherOutCount == _totalWickets;

        /// <summary>
        /// Have both the teams started to bat?. The second half batting team might be still batting.
        /// </summary>
        bool HaveBothPlayersStartedBat() => _gameStateManager.OwnerStartedBat && _gameStateManager.OtherStartedBat;

        /// <summary>
        /// Both teams started batting and Has the owner's score crossed the other's score
        /// </summary>
        bool HasOwnerWon() => HaveBothPlayersStartedBat() && IsOwnerBatting && OwnerTotalScore > OtherTotalScore;

        /// <summary>
        /// Both teams started batting and Has the other team's score crossed the owner's score
        /// </summary>
        bool HasOtherWon() => HaveBothPlayersStartedBat() && !IsOwnerBatting && OtherTotalScore > OwnerTotalScore;

        /// <summary>
        /// [ If the second half batting team is all out OR
        /// If the total overs to be played is limited, then total overs is finished ] AND
        /// Both team scores are same.
        /// </summary>
        bool IsTargetDraw() => (IsSecondHalfBattingTeamAllOut() || HasSecondHalfTotalOversEnded()) && OwnerTotalScore == OtherTotalScore;

        /// <summary>
        /// If both teams have batted AND second half batting team is all out.
        /// </summary>
        bool IsSecondHalfBattingTeamAllOut() => HaveBothPlayersStartedBat() && IsCurrentHalfBattingTeamAllOut();

        /// <summary>
        /// If the game is with limited overs,
        /// Checks if the total overs ended for the current half team, 
        /// </summary>
        bool HasTotalOversOfThisHalfEnded() => HasLimitedOver() && IsLastOverOfThisHalf();

        /// <summary>
        /// If both teams started to bat,
        /// Has the second half batting team finished the total overs to be played.
        /// </summary>
        bool HasSecondHalfTotalOversEnded() => HaveBothPlayersStartedBat() && HasTotalOversOfThisHalfEnded();

        /// <summary>
        /// If this conditions match, we end the game.
        /// </summary>
        bool IsGameEndConditionsMatching() =>
            IsTargetDraw() ||
            HasOwnerWon() ||
            HasOtherWon() ||
            HasSecondHalfTotalOversEnded() ||
            IsSecondHalfBattingTeamAllOut();

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
