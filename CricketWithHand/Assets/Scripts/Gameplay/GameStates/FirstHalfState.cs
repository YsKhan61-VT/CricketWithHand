namespace CricketWithHand.Gameplay
{
    public class FirstHalfState : GameState
    {
        internal override GameStateCategory StateCategory => GameStateCategory.FirstHalf;

        internal FirstHalfState(GameStateManager stateManager) : base(stateManager) {}
    }

}
