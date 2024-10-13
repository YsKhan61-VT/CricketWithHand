namespace CricketWithHand.Gameplay
{
    public class FirstHalfState : GameState
    {
        internal override GameStateCategory StateCategory => GameStateCategory.FirstHalf;

        internal FirstHalfState(GameStateManager stateManager) : base(stateManager) {}

        internal override void Enter()
        {
            stateManager.OnFirstHalfStarted?.Invoke();
        }
    }

}
