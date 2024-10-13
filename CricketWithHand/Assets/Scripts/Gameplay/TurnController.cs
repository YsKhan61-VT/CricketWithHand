using UnityEngine;
using UnityEngine.Events;
using CricketWithHand.Utility;
using System.Threading.Tasks;
using System;


namespace CricketWithHand.Gameplay
{
    public class TurnController : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Invoked when first or second half starts
        /// </summary>
        public UnityEvent OnNextHalfStarted;

        /// <summary>
        /// Invoked when the next over started.
        /// </summary>
        public UnityEvent OnNextOverStarted;

        /// <summary>
        /// Called when the next turn is starting. User input panel needs to be activated.
        /// </summary>
        public UnityEvent OnNextTurnCountdownStarted;

        /// <summary>
        /// Called when the next turn duration ends. User input panel needs to be deactivated.
        /// </summary>
        public UnityEvent OnNextTurnCountdownEnded;

        #endregion

        #region Game Data Containers

        [SerializeField]
        private GameConfigSO _gameConfig;

        [SerializeField]
        private GameDataSO _gameData;

        [SerializeField]
        private PlayerStatsSO _ownerStats;

        [SerializeField]
        private PlayerStatsSO _otherStats;

        #endregion

        [SerializeField]
        GameStateManager _gameStateManager;

        int TotalScore
        {
            get
            {
                if (_gameData.IsOwnerBatting)
                    return _ownerStats.TotalScore.Value;
                else
                    return _otherStats.TotalScore.Value;
            }

            set
            {
                if (_gameData.IsOwnerBatting)
                    _ownerStats.TotalScore.UpdateData(value);
                else
                    _otherStats.TotalScore.UpdateData(value);
            }
        }

        /// <summary>
        /// Use this only in either of First half or Second half state, while a player is batting
        /// </summary>
        int CurrentBallCount
        {
            get
            {
                if (_gameData.IsOwnerBatting)
                    return _ownerStats.BallsCount.Value;
                else
                    return _otherStats.BallsCount.Value;
            }

            set

            {
                if (_gameData.IsOwnerBatting)
                    _ownerStats.BallsCount.UpdateData(value);
                else
                    _otherStats.BallsCount.UpdateData(value);
            }
        }

        /// <summary>
        /// Use this only in either of First half or Second half state, while a player is batting
        /// </summary>
        int CurrentOverCount
        {
            get
            {
                if (_gameData.IsOwnerBatting)
                    return _ownerStats.OversCount.Value;
                else
                    return _otherStats.OversCount.Value;
            }

            set
            {
                if (_gameData.IsOwnerBatting)
                    _ownerStats.OversCount.UpdateData(value);
                else
                    _otherStats.OversCount.UpdateData(value);
            }
        }

        int ScoreInThisBall
        {
            get
            {
                if (_gameData.IsOwnerBatting)
                    return _ownerStats.ScoreInThisBall.Value;
                else
                    return _otherStats.ScoreInThisBall.Value;
            }

            set
            {
                if (_gameData.IsOwnerBatting)
                    _ownerStats.ScoreInThisBall.UpdateData(value);
                else
                    _otherStats.ScoreInThisBall.UpdateData(value);
            }
        }

        int InputScore
        {
            get
            {
                if (_gameData.IsOwnerBatting)
                    return _ownerStats.InputScore.Value;
                else
                    return _otherStats.InputScore.Value;
            }
            
            set
            {
                if (_gameData.IsOwnerBatting)
                    _ownerStats.InputScore.UpdateData(value);
                else
                    _otherStats.InputScore.UpdateData(value);
            }
        }

        int TotalWicketsLost
        {
            get
            {
                if (_gameData.IsOwnerBatting)
                    return _ownerStats.TotalWicketsLost.Value;
                else
                    return _otherStats.TotalWicketsLost.Value;
            }

            set
            {
                if (_gameData.IsOwnerBatting)
                    _ownerStats.TotalWicketsLost.UpdateData(value);
                else
                    _otherStats.TotalWicketsLost.UpdateData(value);
            }
        }

        bool IsOutOnThisBall
        {
            get
            {
                if (_gameData.IsOwnerBatting)
                    return _ownerStats.IsOutInThisBall.Value;
                else
                    return _otherStats.IsOutInThisBall.Value;
            }

            set
            {
                if (_gameData.IsOwnerBatting)
                    _ownerStats.IsOutInThisBall.UpdateData(value);
                else
                    _otherStats.IsOutInThisBall.UpdateData(value);
            }
        }

        Action OnBallPlayed
        {
            get
            {
                if (_gameData.IsOwnerBatting)
                    return _ownerStats.OnBallPlayed;
                else
                    return _otherStats.OnBallPlayed;
            }
        }
       

        float _timeElapsedSinceTurnCountdownStarted = 0;
        bool _pauseCountdown = true;

        void Start()
        {
            _gameData.ResetData();
            _ownerStats.ResetData();
            _otherStats.ResetData();
        }


        void Update()
        {
            TryExecuteNextTurn();
        }

        public void StartHalf()
        {
            if (!PassPreChecks())
            {
                Debug.LogError("Pre Checks not passed!");
                return;
            }

            SetForNewHalf();

            OnNextHalfStarted?.Invoke();

            StartNextOver();

            StartNextTurn();
        }

        public void RegisterOwnerInput(int scoreValue) =>
            _ownerStats.InputScore.UpdateData(scoreValue);

        public void RegisterOtherInput(int scoreValue) =>
            _otherStats.InputScore.UpdateData(scoreValue);

        async void TryExecuteNextTurn()
        {
            if (!HasTurnCountdownEnded())
                return;

            OnNextTurnCountdownEnded?.Invoke();

            CurrentBallCount++;

            CalculateScoreAndWicketsLost();

            OnBallPlayed?.Invoke();

            await Task.Delay(_gameConfig.WaitForSecsBeforeNextTurn * 1000);

            if (IsGameEndConditionsMatching())
            {
                EndGame();
                return;
            }

            if (HasAllOversOfThisHalfEnded() || IsCurrentHalfBattingPlayerAllOut())
            {
                ExecuteHalfTIme();
                return;
            }

            if (IsCurrentOverComplete() && !HasAllOversOfThisHalfEnded())
            {
                StartNextOver();
            }

            StartNextTurn();
        }

        void SetForNewHalf()
        {
            if (_gameData.IsOwnerBatting)
                _ownerStats.StartedBatting.UpdateData(true);
            else
                _otherStats.StartedBatting.UpdateData(true);

            CurrentOverCount = 0;
            CurrentBallCount = 0;
        }

        void ExecuteHalfTIme()
        {
            _gameStateManager.ChangeGameState(GameStateCategory.HalfTime);
        }

        bool HasTurnCountdownEnded()
        {
            if (_pauseCountdown) return false;

            _timeElapsedSinceTurnCountdownStarted += Time.deltaTime;
            _gameData.TurnSliderValue.UpdateData(1 - (_timeElapsedSinceTurnCountdownStarted / _gameConfig.TurnDurationInSecs));

            if (_timeElapsedSinceTurnCountdownStarted < _gameConfig.TurnDurationInSecs)
                return false;
            else
            {
                _pauseCountdown = true;
                return true;
            }
        }

        void StartNextOver()
        {
            CurrentOverCount++;
            OnNextOverStarted?.Invoke();
        }

        void StartNextTurn()
        {
            _timeElapsedSinceTurnCountdownStarted = 0;
            _gameData.TurnSliderValue.UpdateData(1);
            _pauseCountdown = false;
            
            _ownerStats.InputScore.UpdateData(0);
            _otherStats.InputScore.UpdateData(0);

            IsOutOnThisBall = false;

            OnNextTurnCountdownStarted?.Invoke();
        }

        void EndGame()
        {
            _gameStateManager.ChangeGameState(GameStateCategory.GameEnd);
        }

        void CalculateScoreAndWicketsLost()
        {
            // Check if is out
            IsOutOnThisBall = _ownerStats.InputScore.Value == _otherStats.InputScore.Value;

            ScoreInThisBall = IsOutOnThisBall ? 0 : _gameData.IsOwnerBatting ? _ownerStats.InputScore.Value : _otherStats.InputScore.Value;
            TotalScore += ScoreInThisBall;

            if (!IsOutOnThisBall)
                return;

            TotalWicketsLost++;
        }

        /// <summary>
        /// Is the total balls of the current over completed playing.
        /// </summary>
        bool IsCurrentOverComplete() => CurrentBallCount == _gameConfig.BallsPerOver;

        /// <summary>
        /// Is this over the last over of this half
        /// </summary>
        bool IsThisLastOverOfThisHalf() => CurrentOverCount == _gameData.TotalOversCountDataContainer.Value;

        /// <summary>
        /// Has the current half batting player lost all wickets.
        /// </summary>
        bool IsCurrentHalfBattingPlayerAllOut() => TotalWicketsLost == _gameData.TotalWicketsCountDataContainer.Value;

        /// <summary>
        /// Is this second half running
        /// </summary>
        bool IsSecondHalf() => _gameData.CurrentGameStateCategory.Value == GameStateCategory.SecondHalf;

        /// <summary>
        /// If both teams have batted AND second half batting team is all out.
        /// </summary>
        bool IsSecondHalfBattingPlayerAllOut() => IsSecondHalf() && IsCurrentHalfBattingPlayerAllOut();

        /// <summary>
        /// has all the overs of this half been played
        /// </summary>
        bool HasAllOversOfThisHalfEnded() => IsCurrentOverComplete() && IsThisLastOverOfThisHalf();

        /// <summary>
        /// Has all the overs of the second half been played
        /// </summary>
        bool HasAllOversOfSecondHalfEnded() => IsSecondHalf() && HasAllOversOfThisHalfEnded();

        /// <summary>
        /// Both teams started batting and Has the owner's score crossed the other's score
        /// </summary>
        bool HasOwnerWon()
        {
            bool won = IsSecondHalf() && 
                _ownerStats.TotalScore.Value > _otherStats.TotalScore.Value;
            if (won)
                _gameData.Winner.UpdateData(PlayerType.OWNER);
            return won;
        }

        /// <summary>
        /// Both teams started batting and Has the other team's score crossed the owner's score
        /// </summary>
        bool HasOtherWon()
        {
            bool won = IsSecondHalf() &&
                _ownerStats.TotalScore.Value < _otherStats.TotalScore.Value;
            if (won)
                _gameData.Winner.UpdateData(PlayerType.OTHER);
            return won;
        }

        /// <summary>
        /// Is the game a draw
        /// </summary>
        bool IsTargetDraw()
        {
            bool isDraw = (IsSecondHalfBattingPlayerAllOut() || HasAllOversOfSecondHalfEnded()) &&
                _ownerStats.TotalScore.Value == _otherStats.TotalScore.Value;
            if (isDraw)
                _gameData.Winner.UpdateData(PlayerType.NONE);
            return isDraw;
        }

        /// <summary>
        /// If this conditions match, we end the game.
        /// </summary>
        bool IsGameEndConditionsMatching() =>
            IsTargetDraw() ||
            HasOwnerWon() ||
            HasOtherWon() ||
            HasAllOversOfSecondHalfEnded() ||
            IsSecondHalfBattingPlayerAllOut();

        bool PassPreChecks()
        {
            if (_gameConfig.BallsPerOver == 0 || _gameData.TotalOversCountDataContainer.Value == 0)
            {
                gameObject.SetActive(false);
                return false;
            }
            return true;
        }
    }
}
