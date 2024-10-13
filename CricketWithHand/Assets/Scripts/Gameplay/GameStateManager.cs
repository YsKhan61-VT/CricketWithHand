using UnityEngine;
using UnityEngine.Events;


namespace CricketWithHand.Gameplay
{
    public class GameStateManager : MonoBehaviour
    {
        #region Events

        [SerializeField]
        internal UnityEvent OnFirstHalfStarted;

        [SerializeField]
        internal UnityEvent OnHalfTimeStarted;

        [SerializeField]
        internal UnityEvent OnSecondHalfStarted;

        [SerializeField]
        internal UnityEvent OnGameEnds;

        #endregion

        [SerializeField]
        GameStateCategoryDataContainerSO _currentGameStateCategory;

        private GameState _firstHalfState;
        private GameState _halfTimeState;
        private GameState _secondHalfState;
        private GameState _gameEndState;

        private GameState _currentGameState;

        private void Awake()
        {
            _firstHalfState = new FirstHalfState(this);
            _halfTimeState = new HalfTimeState(this);
            _secondHalfState = new SecondHalfState(this);
            _gameEndState = new GameEndState(this);
        }

        public void StartFirstHalf() => ChangeGameState(GameStateCategory.FirstHalf);

        public void StartSecondHalf() => ChangeGameState(GameStateCategory.SecondHalf);

        public void ChangeGameState(GameStateCategory state)
        {
            _currentGameState?.Exit();
            _currentGameState = ChooseState(state);
            _currentGameStateCategory.UpdateData(_currentGameState.StateCategory);
            _currentGameState?.Enter();
        }

        private GameState ChooseState(GameStateCategory state)
        {
            switch (state)
            {
                case GameStateCategory.FirstHalf:
                    return _firstHalfState;

                case GameStateCategory.HalfTime:
                    return _halfTimeState;

                case GameStateCategory.SecondHalf:
                    return _secondHalfState;

                case GameStateCategory.GameEnd:
                    return _gameEndState;
            }

            Debug.LogError("This should not happen!");
            return null;
        }
    }

}
