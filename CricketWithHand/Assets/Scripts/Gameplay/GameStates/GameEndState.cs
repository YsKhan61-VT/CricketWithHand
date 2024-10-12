using System;

namespace CricketWithHand.Gameplay
{
    public class GameEndState : GameState
    {
        internal override GameStateCategory StateCategory => GameStateCategory.GameEnd;

        private TurnController _turnController;

        internal GameEndState(
            GameStateManager gameStateManager, 
            TurnController turnController) : base(gameStateManager)
        {
            _turnController = turnController;
        }

        internal override void Enter()
        {
            StateManager.OnGameEnds?.Invoke();
            _turnController.ProcessResult();
        }
    }

}
