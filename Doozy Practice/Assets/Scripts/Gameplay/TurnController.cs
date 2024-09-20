using System.Threading.Tasks;
using UnityEngine;


namespace DoozyPractice.Gameplay
{
    public enum InputScore : int
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6
    }

    public class TurnController : MonoBehaviour
    {
        private const int BALLS_PER_OVER = 6;

        [SerializeField]
        GameStateManager _gameStateManager;

        [SerializeField]
        GameplayUIMediator _gameplayUIMediator;

        [SerializeField]
        int _totalOvers;

        [SerializeField, Tooltip("Duration for each turn in seconds")]
        int _turnDuration;

        public bool IsOwnerBatting => _gameStateManager.CurrentGameState.StateCategory == GameStateCategory.Owner_Batting;

        private int _currentBallCount;
        private int _currentOverCount;
        private float _timeElapsedSinceLastTurn;

        private int _ownerTurnScore;
        private int _otherTurnScore;

        public int OwnerTotalScore { get; private set; }
        public int OtherTotalScore { get; private set; }

        private void Start()
        {
            ResetAtStart();
            _gameStateManager.ChangeGameState(GameStateCategory.Owner_Batting);
        }

        private void Update()
        {
            TryExecuteTurn();
        }

        public void OwnerGaveInput(int scoreValue)
        {
            _ownerTurnScore = scoreValue;
        }

        public void OtherGaveInput(int scoreValue)
        {
            _otherTurnScore = scoreValue;
        }

        async void TryExecuteTurn()
        {
            _timeElapsedSinceLastTurn += Time.deltaTime;
            _gameplayUIMediator.UpdateTurnDurationSlider(1 - (_timeElapsedSinceLastTurn/ _turnDuration));
            if (_timeElapsedSinceLastTurn < _turnDuration)
                return;

            CalculateScore();
            UpdateScoreUI();

            await Task.Delay(1000);

            if (HaveBothPlayersBatted())
            {
                EndGame();
            }

            ResetForNextTurn();

            if (HasCurrentOverEnded())
            {
                ResetForNextOver();
            }

            if (HaveWholeOversEnded())
            {
                ChangeBatsman();
            }
        }

        void ResetForNextTurn()
        {
            _timeElapsedSinceLastTurn = 0;
            _gameplayUIMediator.UpdateTurnDurationSlider(1);
        }

        void ResetForNextOver()
        {
            _gameplayUIMediator.ResetOverScoreUI();
        }

        void ChangeBatsman()
        {
            if (IsOwnerBatting)
            {
                _gameStateManager.ChangeGameState(GameStateCategory.Other_Batting);
            }
            else
            {
                _gameStateManager.ChangeGameState(GameStateCategory.Owner_Batting);
            }
        }

        void EndGame()
        {
            _gameStateManager.ChangeGameState(GameStateCategory.GameEnd);
        }

        void CalculateScore()
        {
            int score = Mathf.Abs(_ownerTurnScore - _otherTurnScore);
            if (IsOwnerBatting)
            {
                _ownerTurnScore = score;
                OwnerTotalScore += score;
            }
            else
            {
                _otherTurnScore = score;
                OtherTotalScore += score;
            }
        }

        void UpdateScoreUI()
        {
            _gameplayUIMediator.UpdateOwnerTurnScoreUI(_ownerTurnScore);
            _gameplayUIMediator.UpdateOtherTurnScoreUI(_otherTurnScore);

            if (IsOwnerBatting)
            {
                _gameplayUIMediator.UpdateOwnerTotalScoreUI(OwnerTotalScore);
                _gameplayUIMediator.UpdateOverScoreUI(_currentBallCount, _ownerTurnScore);
            }
            else
            {
                _gameplayUIMediator.UpdateOtherTotalScoreUI(OtherTotalScore);
                _gameplayUIMediator.UpdateOverScoreUI(_currentBallCount, _otherTurnScore);
            }
        }

        bool HasCurrentOverEnded() => _currentBallCount >= BALLS_PER_OVER;

        bool HaveWholeOversEnded() => _currentOverCount >= _totalOvers;

        bool HaveBothPlayersBatted() => _gameStateManager.OwnerDidBat && _gameStateManager.OtherDidbat;
    
        void ResetAtStart()
        {
            OwnerTotalScore = 0;
            OtherTotalScore = 0;
            _currentBallCount = 0;
            _currentOverCount = 0;
            _timeElapsedSinceLastTurn = 0;
            _ownerTurnScore = 0;
            _otherTurnScore = 0;
            _gameplayUIMediator.UpdateTurnDurationSlider(1);
        }
    }
}
