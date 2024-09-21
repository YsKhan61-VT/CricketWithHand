using System;

namespace DoozyPractice.Gameplay
{
    public class GameEndState : GameState
    {
        public override GameStateCategory StateCategory => GameStateCategory.GameEnd;

        private TurnController _turnController;
        private GameplayUIMediator _gameplayUIMediator;

        public GameEndState(
            GameStateManager gameStateManager, 
            TurnController turnController,
            GameplayUIMediator gameplayUIMediator) : base(gameStateManager)
        {
            _turnController = turnController;
            _gameplayUIMediator = gameplayUIMediator;
        }

        public override void Enter()
        {
            StateManager.OnGameEnds?.Invoke();
            ProcessResult();
        }

        void ProcessResult()
        {
            int scoreDiff = _turnController.OtherTotalScore - _turnController.OwnerTotalScore;

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
    }

}
