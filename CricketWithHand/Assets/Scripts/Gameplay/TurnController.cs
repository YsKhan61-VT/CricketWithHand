using UnityEngine;
using UnityEngine.Events;
using CricketWithHand.Utility;
using System.Threading.Tasks;
using Doozy.Runtime.Mody.Modules;


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
        GameConfigSO _gameConfig;

        #region Owner Data Containers

        /// <summary>
        /// Invoked everytime owner gives input
        /// </summary>
        [SerializeField]
        IntDataContainerSO _ownerInputScoreContainer;

        /// <summary>
        /// Called everytime owner is playing a new over
        /// </summary>
        [SerializeField]
        IntDataContainerSO _ownerOverCountContainer;

        /// <summary>
        /// Called everytime owner is playing a new ball
        /// </summary>
        [SerializeField]
        IntDataContainerSO _ownerBallCountContainer;

        [SerializeField]
        IntDataContainerSO _ownerTurnScoreContainer;

        /*[SerializeField]
        BoolDataContainerSO _ownerIsOutOnThisTurnContainer;*/

        /// <summary>
        /// Called everytime owner played a new ball
        /// </summary>
        [SerializeField]
        BallDataContainerSO _ownerBallDataContainer;

        /// <summary>
        /// Called everytime the owner total score gets updated.
        /// </summary>
        [SerializeField]
        IntDataContainerSO _ownerTotalScoreContainer;

        [SerializeField]
        IntDataContainerSO _ownerTotalWicketLostContainer;

        #endregion

        #region Other Data Containers

        /// <summary>
        /// Invoked everytime owner gives input
        /// </summary>
        [SerializeField]
        IntDataContainerSO _otherInputScoreContainer;

        /// <summary>
        /// Called everytime other player is playing a new over
        /// </summary>
        [SerializeField]
        IntDataContainerSO _otherOverCountContainer;

        /// <summary>
        /// Called everytime other player is playing a new ball
        /// </summary>
        [SerializeField]
        IntDataContainerSO _otherBallCountContainer;

        [SerializeField]
        IntDataContainerSO _otherTurnScoreContainer;

        /// <summary>
        /// Called everytime other played a new ball
        /// </summary>
        [SerializeField]
        BallDataContainerSO _otherBallDataContainer;

        /// <summary>
        /// Called everytime the other player's total score gets updated.
        /// </summary>
        [SerializeField]
        IntDataContainerSO _otherTotalScoreContainer;

        [SerializeField]
        IntDataContainerSO _otherTotalWicketLostContainer;

        #endregion

        [SerializeField]
        BatsmanDataContainerSO _batsmanOfFirstHalf;

        [SerializeField]
        IntDataContainerSO _totalOversCountDataContainer;

        [SerializeField]
        IntDataContainerSO _totalWicketsCountDataContainer;

        #endregion

        [SerializeField]
        GameStateManager _gameStateManager;

        [SerializeField]
        GameplayUIMediator _gameplayUIMediator;


        bool IsOwnerBatting
        {
            get
            {
                // If this is first half and batsman of first half is owner,
                // OR
                // If this is second half and batsman of fist half was other

                return (_gameStateManager.CurrentGameState.StateCategory == GameStateCategory.FirstHalf &&
                    _batsmanOfFirstHalf.Value == PlayerType.OWNER) ||
                (_gameStateManager.CurrentGameState.StateCategory == GameStateCategory.SecondHalf &&
                    _batsmanOfFirstHalf.Value == PlayerType.OTHER);
            }
        }

        // public int OwnerTotalScore { get; private set; }
        // public int OtherTotalScore { get; private set; }

        int _totalScore
        {
            get
            {
                if (IsOwnerBatting)
                    return _ownerTotalScoreContainer.Value;
                else
                    return _otherTotalScoreContainer.Value;
            }

            set
            {
                if (IsOwnerBatting)
                    _ownerTotalScoreContainer.UpdateData(value);
                else
                    _otherTotalScoreContainer.UpdateData(value);
            }
        }

        /// <summary>
        /// Use this only in either of First half or Second half state, while a player is batting
        /// </summary>
        int _currentBallCount
        {
            get
            {
                if (IsOwnerBatting)
                    return _ownerBallCountContainer.Value;
                else
                    return _otherBallCountContainer.Value;
            }

            set

            {
                if (IsOwnerBatting)
                    _ownerBallCountContainer.UpdateData(value);
                else
                    _otherBallCountContainer.UpdateData(value);
            }
        }

        /// <summary>
        /// Use this only in either of First half or Second half state, while a player is batting
        /// </summary>
        int _currentOverCount
        {
            get
            {
                if (IsOwnerBatting)
                    return _ownerOverCountContainer.Value;
                else
                    return _otherOverCountContainer.Value;
            }

            set
            {
                if (IsOwnerBatting)
                    _ownerTotalScoreContainer.UpdateData(value);
                else
                    _otherTotalScoreContainer.UpdateData(value);
            }
        }

        int _turnScore
        {
            get
            {
                if (IsOwnerBatting)
                    return _ownerTurnScoreContainer.Value;
                else
                    return _otherTurnScoreContainer.Value;
            }

            set
            {
                if (IsOwnerBatting)
                    _ownerTurnScoreContainer.UpdateData(value);
                else
                    _otherTurnScoreContainer.UpdateData(value);
            }
        }

        BallData _turnData
        {
            get
            {
                if (IsOwnerBatting)
                    return _ownerBallDataContainer.Value;
                else
                    return _otherBallDataContainer.Value;
            }

            set
            {
                if (IsOwnerBatting)
                    _ownerBallDataContainer.UpdateData(value);
                else
                    _otherBallDataContainer.UpdateData(value);
            }
        }

        int _inputScore
        {
            get
            {
                if (IsOwnerBatting)
                    return _ownerInputScoreContainer.Value;
                else
                    return _otherInputScoreContainer.Value;
            }
            
            set
            {
                if (IsOwnerBatting)
                    _ownerInputScoreContainer.UpdateData(value);
                else
                    _otherInputScoreContainer.UpdateData(value);
            }
        }

        int _totalWicketsLost
        {
            get
            {
                if (IsOwnerBatting)
                    return _ownerTotalWicketLostContainer.Value;
                else
                    return _otherTotalWicketLostContainer.Value;
            }

            set
            {
                if (IsOwnerBatting)
                    _ownerTotalWicketLostContainer.UpdateData(value);
                else
                    _otherTotalWicketLostContainer.UpdateData(value);
            }
        }
       

        float _timeElapsedSinceTurnCountdownStarted = 0;
        bool _isOutOnThisTurn = false;
        bool _pauseCountdown = true;

        void Start()
        {
            ResetAtStart();
        }


        void Update()
        {
            TryExecuteNextTurn();
        }

        /*public void ChangeBatsman(bool isOwnerBatting)
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
        }*/

        public void StartHalf()
        {
            if (!PassPreChecks()) return;

            ResetForNewHalf();

            OnNextHalfStarted?.Invoke();

            IncreaseOverCount();

            StartNextTurn();
        }

        public void RegisterOwnerInput(int scoreValue) =>
            _ownerInputScoreContainer.UpdateData(scoreValue);

        public void RegisterOtherInput(int scoreValue) =>
            _otherInputScoreContainer.UpdateData(scoreValue);

        /*public void StartNextHalf()
        {
            ChangeBatsman(!IsOwnerBatting);
            _currentOverCount = 0;
            // _gameplayUIMediator.ResetOverScoreUI();
            _currentBallCount = 0;
            IncreaseOverCount();
            StartNextTurn();
        }*/

        public void ProcessResult()
        {
            int scoreDiff = _otherTotalScoreContainer.Value - _ownerTotalScoreContainer.Value;

            switch (scoreDiff)
            {
                case 0:
                    _gameplayUIMediator.ShowDrawText();
                    break;

                case > 0:
                    _gameplayUIMediator.ShowLossText();
                    break;

                case < 0:
                    _gameplayUIMediator.ShowWinText();
                    break;
            }
        }

        async void TryExecuteNextTurn()
        {
            if (!HasTurnCountdownEnded())
                return;

            OnNextTurnCountdownEnded?.Invoke();

            _currentBallCount++;

            CalculateScoreAndWicketsLost();
            UpdateScoreboardUIs();

            await Task.Delay(_gameConfig.WaitForSecsBeforeNextTurn * 1000);

            if (IsGameEndConditionsMatching())
            {
                EndGame();
                return;
            }

            if (HasLastOverOfThisHalfEnded() || IsCurrentHalfBattingTeamAllOut())
            {
                ExecuteHalfTIme();
                return;
            }

            if (IsCurrentOverComplete() && !HasLastOverOfThisHalfEnded())
            {
                // _currentOverCount++;
                IncreaseOverCount();
            }

            StartNextTurn();
        }

        void IncreaseOverCount()
        {
            _currentOverCount++;
            if (IsOwnerBatting)
            {
                _ownerOverCountContainer.UpdateData(_ownerOverCountContainer.Value + 1);
                // _ownerGameStats.AddNewOver();
            }
            else
            {
                _otherOverCountContainer.UpdateData(_otherOverCountContainer.Value + 1);
                // _otherGameStats.AddNewOver();
            }
        }

        void ResetForNewHalf()
        {
            _currentOverCount = 0;
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
            _gameplayUIMediator.UpdateTurnDurationSlider(1 - (_timeElapsedSinceTurnCountdownStarted / _gameConfig.TurnDurationInSecs));

            if (_timeElapsedSinceTurnCountdownStarted < _gameConfig.TurnDurationInSecs)
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
            
            _ownerInputScoreContainer.UpdateData(0);
            _otherInputScoreContainer.UpdateData(0);
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
            _isOutOnThisTurn = _ownerInputScoreContainer.Value == _otherInputScoreContainer.Value;

            _turnScore = _isOutOnThisTurn ? 0 : IsOwnerBatting ? _ownerInputScoreContainer.Value : _otherInputScoreContainer.Value;
            _totalScore += _turnScore;
            _turnData = new BallData
            {
                OverNumber = _currentOverCount,
                BallNumber = _currentBallCount,
                Score = _turnScore,
                IsWicketLost = _isOutOnThisTurn,
            };

            // Calculate score
            /*if (IsOwnerBatting)
            {
                OwnerTotalScore += _turnScore;
                _ownerTotalScoreContainer.UpdateData(OwnerTotalScore);
                _ownerBallDataContainer.UpdateData(new BallData
                {
                    OverNumber = _ownerOverCountContainer.Value,
                    BallNumber = _currentBallCount,
                    Score = _turnScore,
                    IsWicketLost = _isOutOnThisTurn,
                });
            }
                
            else
            {
                OtherTotalScore += _turnScore;
                _otherTotalScoreContainer.UpdateData(OtherTotalScore);
                _otherBallDataContainer.UpdateData(new BallData
                {
                    OverNumber = _otherOverCountContainer.Value,
                    BallNumber = _currentBallCount,
                    Score = _turnScore,
                    IsWicketLost = _isOutOnThisTurn,
                });
            }*/

            if (!_isOutOnThisTurn)
                return;

            _totalWicketsLost++;

            // Calculate wicket
            /*if (IsOwnerBatting)
                _ownerOutCount++;
            else
                _otherOutCount++;*/
        }

        void UpdateScoreboardUIs()
        {
            _gameplayUIMediator.UpdateOwnerInputScoreUI(_ownerInputScoreContainer.Value);
            _gameplayUIMediator.UpdateOtherInputScoreUI(_otherInputScoreContainer.Value);

            if (IsOwnerBatting)
            {
                // _gameplayUIMediator.UpdateOwnerTotalScoreUI(OwnerTotalScore);
                _gameplayUIMediator.UpdateOwnerTotalWicketsUI(_totalWicketsLost, _totalWicketsCountDataContainer.Value);
                _gameplayUIMediator.UpdateOwnerTotalOversUI(_currentOverCount-1, _currentBallCount, _totalOversCountDataContainer.Value);
            }
            else
            {
                // _gameplayUIMediator.UpdateOtherTotalScoreUI(OtherTotalScore);
                _gameplayUIMediator.UpdateOtherTotalWicketsUI(_totalWicketsLost, _totalWicketsCountDataContainer.Value);
                _gameplayUIMediator.UpdateOtherTotalOversUI(_currentOverCount-1, _currentBallCount, _totalOversCountDataContainer.Value);
            }

            _gameplayUIMediator.UpdateOverScoreUI(_currentBallCount, _turnScore, _isOutOnThisTurn);
        }

        /// <summary>
        /// Is the total balls of the current over completed playing.
        /// </summary>
        bool IsCurrentOverComplete() => _currentBallCount == _gameConfig.BallsPerOver;

        /// <summary>
        /// Has the current half batting team lost all wickets.
        /// </summary>
        bool IsCurrentHalfBattingTeamAllOut() => 
            IsOwnerBatting ? 
            _totalWicketsLost == _totalWicketsCountDataContainer.Value : 
            _totalWicketsLost == _totalWicketsCountDataContainer.Value;

        /// <summary>
        /// Have both the teams started to bat?. The second half batting team might be still batting.
        /// </summary>
        bool HaveBothPlayersStartedBat() => _gameStateManager.OwnerStartedBat && _gameStateManager.OtherStartedBat;

        /// <summary>
        /// Is the current over the last over of this half(1st half or 2nd half)
        /// </summary>
        bool HasLastOverOfThisHalfEnded() => IsCurrentOverComplete() && _currentOverCount == _totalOversCountDataContainer.Value;

        /// <summary>
        /// Both teams started batting and Has the owner's score crossed the other's score
        /// </summary>
        bool HasOwnerWon() => HaveBothPlayersStartedBat() && IsOwnerBatting && _ownerTotalScoreContainer.Value > _otherTotalScoreContainer.Value;

        /// <summary>
        /// Both teams started batting and Has the other team's score crossed the owner's score
        /// </summary>
        bool HasOtherWon() => HaveBothPlayersStartedBat() && !IsOwnerBatting && _otherTotalScoreContainer.Value > _ownerTotalScoreContainer.Value;

        /// <summary>
        /// [ If the second half batting team is all out OR
        /// If the total overs to be played is limited, then total overs is finished ] AND
        /// Both team scores are same.
        /// </summary>
        bool IsTargetDraw() => (IsSecondHalfBattingTeamAllOut() || HasSecondHalfTotalOversEnded()) && _ownerTotalScoreContainer.Value == _otherTotalScoreContainer.Value;

        /// <summary>
        /// If both teams have batted AND second half batting team is all out.
        /// </summary>
        bool IsSecondHalfBattingTeamAllOut() => HaveBothPlayersStartedBat() && IsCurrentHalfBattingTeamAllOut();

        /// <summary>
        /// If both teams started to bat,
        /// Has the second half batting team finished the total overs to be played.
        /// </summary>
        bool HasSecondHalfTotalOversEnded() => HaveBothPlayersStartedBat() && HasLastOverOfThisHalfEnded();

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
            if (_gameConfig.BallsPerOver == 0 || _totalOversCountDataContainer.Value == 0)
            {
                gameObject.SetActive(false);
                return false;
            }
            return true;
        }

        void ResetAtStart()
        {
            // _gameplayUIMediator.ResetOverScoreUI();
            // _gameplayUIMediator.UpdateOwnerTotalScoreUI(0);
            // _gameplayUIMediator.UpdateOtherTotalScoreUI(0);
            _gameplayUIMediator.UpdateOwnerTotalWicketsUI(0, 0);
            _gameplayUIMediator.UpdateOtherTotalWicketsUI(0, 0);
            _gameplayUIMediator.UpdateOwnerTotalOversUI(0, 0, 0);
            _gameplayUIMediator.UpdateOtherTotalOversUI(0, 0, 0);

            _ownerOverCountContainer.UpdateData(0);
            _otherOverCountContainer.UpdateData(0);

            _ownerTotalScoreContainer.UpdateData(0);
            _otherTotalScoreContainer.UpdateData(0);
        }
    }
}
