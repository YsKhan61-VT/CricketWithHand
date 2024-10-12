using CricketWithHand.Utility;
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

        #region Events

        /// <summary>
        /// Called when the next turn is starting. User input panel needs to be activated.
        /// </summary>
        public UnityEvent OnNextTurnCountdownStarted;

        /// <summary>
        /// Called when the next turn duration ends. User input panel needs to be deactivated.
        /// </summary>
        public UnityEvent OnNextTurnCountdownEnded;

        /*/// <summary>
        /// Called when the total overs to be played in each half is set first time.
        /// </summary>
        public UnityEvent<int> OnTotalGameOversSet;*/

        #endregion

        [SerializeField]
        GameConfigSO _gameConfig;

        /// <summary>
        /// Called everytime owner is playing a new over
        /// </summary>
        [SerializeField]
        IntDataContainerSO _ownerOverCountContainer;

        /// <summary>
        /// Called everytime other player is playing a new over
        /// </summary>
        [SerializeField]
        IntDataContainerSO _otherOverCountContainer;

        /// <summary>
        /// Called everytime owner played a new ball
        /// </summary>
        [SerializeField]
        BallDataContainerSO _ownerBallDataContainer;

        /// <summary>
        /// Called everytime other played a new ball
        /// </summary>
        [SerializeField]
        BallDataContainerSO _otherBallDataContainer;

        /// <summary>
        /// Called everytime the owner total score gets updated.
        /// </summary>
        [SerializeField]
        IntDataContainerSO _ownerTotalScoreContainer;

        /// <summary>
        /// Called everytime the other player's total score gets updated.
        /// </summary>
        [SerializeField]
        IntDataContainerSO _otherTotalScoreContainer;

        [SerializeField]
        IntDataContainerSO _totalOversCountDataContainer;

        [SerializeField]
        IntDataContainerSO _totalWicketsCountDataContainer;

        /*[SerializeField]
        GameStatsSO _ownerGameStats;

        [SerializeField]
        GameStatsSO _otherGameStats;*/

        [SerializeField]
        GameStateManager _gameStateManager;

        [SerializeField]
        GameplayUIMediator _gameplayUIMediator;

        /*[SerializeField]
        int _ballsPerOver;*/

        /*[SerializeField]
        int _totalOvers;*/
        

        /*[SerializeField]
        int _totalWickets;*/

        /*[SerializeField, Tooltip("Duration for each turn in seconds")]
        int _turnDuration;

        [SerializeField, Tooltip("Wait for seconds, before starting next turn")]
        int _waitBeforeNextTurn;*/

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
            ResetAtStart();
        }


        void Update()
        {
            TryExecuteNextTurn();
        }

        /*public void SetTotalOvers(OverCategory overCategory)
        {
            // _totalOvers = (int)overCategory;
            _totalOversContainer.UpdateData((int)overCategory);

            // OnTotalGameOversSet?.Invoke(_totalOvers);

            // _gameplayUIMediator.UpdateOwnerTotalOversUI(0, 0, _totalOvers);
            // _gameplayUIMediator.UpdateOtherTotalOversUI(0, 0, _totalOvers);
        }*/

        /*public void SetTotalWickets(int count)
        {
            _totalWickets = count;

            _gameplayUIMediator.UpdateOwnerTotalWicketsUI(0, _totalWickets);
            _gameplayUIMediator.UpdateOtherTotalWicketsUI(0, _totalWickets);
        }*/

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

            IncreaseOverCount();

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
            IncreaseOverCount();
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

            await Task.Delay(_gameConfig.WaitForSecsBeforeNextTurn * 1000);

            if (IsGameEndConditionsMatching())
            {
                EndGame();
                return;
            }

            if (HasTotalOversOfThisHalfEnded() || IsCurrentHalfBattingTeamAllOut())
            {
                ExecuteHalfTIme();
                return;
            }

            if (IsCurrentOverComplete() && !HasTotalOversOfThisHalfEnded())
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

            ResetForNewOver();
        }

        void ResetForNewOver()
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
            }

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
                // _gameplayUIMediator.UpdateOwnerTotalScoreUI(OwnerTotalScore);
                _gameplayUIMediator.UpdateOwnerTotalWicketsUI(_ownerOutCount, _totalWicketsCountDataContainer.Value);
                _gameplayUIMediator.UpdateOwnerTotalOversUI(_currentOverCount-1, _currentBallCount, _totalOversCountDataContainer.Value);
            }
            else
            {
                // _gameplayUIMediator.UpdateOtherTotalScoreUI(OtherTotalScore);
                _gameplayUIMediator.UpdateOtherTotalWicketsUI(_otherOutCount, _totalWicketsCountDataContainer.Value);
                _gameplayUIMediator.UpdateOtherTotalOversUI(_currentOverCount-1, _currentBallCount, _totalOversCountDataContainer.Value);
            }

            _gameplayUIMediator.UpdateOverScoreUI(_currentBallCount, _turnScore, _isOutOnThisTurn);
        }

        /// <summary>
        /// Is the total balls of the current over completed playing.
        /// </summary>
        bool IsCurrentOverComplete() => _currentBallCount == _gameConfig.BallsPerOver;

        /// <summary>
        /// Does the game have limited overs to play or until all wickets are lost.
        /// </summary>
        bool HasLimitedOver() => _totalOversCountDataContainer.Value != (int)OverCategory.AllOut;

        /// <summary>
        /// Has the current half batting team lost all wickets.
        /// </summary>
        bool IsCurrentHalfBattingTeamAllOut() => 
            IsOwnerBatting ? 
            _ownerOutCount == _totalWicketsCountDataContainer.Value : 
            _otherOutCount == _totalWicketsCountDataContainer.Value;

        /// <summary>
        /// Have both the teams started to bat?. The second half batting team might be still batting.
        /// </summary>
        bool HaveBothPlayersStartedBat() => _gameStateManager.OwnerStartedBat && _gameStateManager.OtherStartedBat;

        /// <summary>
        /// Is the current over the last over of this half(1st half or 2nd half)
        /// </summary>
        bool IsLastOverOfThisHalf() => IsCurrentOverComplete() && _currentOverCount == _totalOversCountDataContainer.Value;

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
            if (_gameConfig.BallsPerOver == 0 || _totalOversCountDataContainer.Value == 0)
            {
                gameObject.SetActive(false);
                return false;
            }
            return true;
        }

        void ResetAtStart()
        {
            _gameplayUIMediator.ResetOverScoreUI();
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
