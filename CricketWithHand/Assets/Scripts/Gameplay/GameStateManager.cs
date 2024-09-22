using UnityEngine;
using UnityEngine.Events;


namespace DoozyPractice.Gameplay
{
    public enum GameStateCategory
    {
        Owner_Batting,
        HalfTime,
        Other_Batting,
        GameEnd
    }

    public class GameStateManager : MonoBehaviour
    {
        /// <summary>
        /// Invoked when the Owner starts batting
        /// </summary>
        public UnityEvent OnOwnerBattingStateStarted;

        /// <summary>
        /// Invoked when the Other client starts batting (can be AI or other client)
        /// </summary>
        public UnityEvent OnOtherBattingStateStarted;

        /// <summary>
        /// Invoked when half time is started.
        /// </summary>
        public UnityEvent OnHalfTimeStarted;

        /// <summary>
        /// Invoked when both the players finished batting.
        /// </summary>
        public UnityEvent OnGameEnds;

        [SerializeField]
        GameplayUIMediator _gameplayUIMediator;

        [SerializeField]
        TurnController _turnController;

        public GameState CurrentGameState { get; private set; }
        public bool OwnerDidBatting { get; set; } = false;
        public bool OtherDidBatting { get; set; } = false;

        private GameState _ownerBattingState;
        private GameState _halfTimeState;
        private GameState _otherBattingState;
        private GameState _gameEndState;

        private void Awake()
        {
            _ownerBattingState = new OwnerBattingState(this, _gameplayUIMediator);
            _halfTimeState = new HalfTimeState(this);
            _otherBattingState = new OtherBattingState(this, _gameplayUIMediator);
            _gameEndState = new GameEndState(this, _turnController, _gameplayUIMediator);
        }

        public void ChangeGameState(GameStateCategory state)
        {
            CurrentGameState?.Exit();
            CurrentGameState = ChooseState(state);
            CurrentGameState?.Enter();
        }

        public GameState ChooseState(GameStateCategory state)
        {
            switch (state)
            {
                case GameStateCategory.Owner_Batting:
                    return _ownerBattingState;

                case GameStateCategory.HalfTime:
                    return _halfTimeState;

                case GameStateCategory.Other_Batting:
                    return _otherBattingState;

                case GameStateCategory.GameEnd:
                    return _gameEndState;
            }

            Debug.LogError("This should not happen!");
            return null;
        }
    }

}
