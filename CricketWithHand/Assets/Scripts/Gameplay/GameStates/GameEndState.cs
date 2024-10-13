using System;

namespace CricketWithHand.Gameplay
{
    public class GameEndState : GameState
    {
        internal override GameStateCategory StateCategory => GameStateCategory.GameEnd;

        internal GameEndState(GameStateManager gameStateManager) : base(gameStateManager) { }

        internal override void Enter()
        {
            stateManager.OnGameEnds?.Invoke();
        }
    }
}
