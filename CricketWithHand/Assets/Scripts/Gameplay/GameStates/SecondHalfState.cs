namespace CricketWithHand.Gameplay
{
    public class SecondHalfState : GameState
    {
        internal override GameStateCategory StateCategory => GameStateCategory.SecondHalf;

        internal SecondHalfState(GameStateManager stateManager) : base(stateManager) {}

        internal override void Enter()
        {
            stateManager.OnSecondHalfStarted?.Invoke();
        }
    }

}
