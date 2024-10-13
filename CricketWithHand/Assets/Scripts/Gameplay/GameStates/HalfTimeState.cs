namespace CricketWithHand.Gameplay
{
    public class HalfTimeState : GameState
    {
        internal override GameStateCategory StateCategory => GameStateCategory.HalfTime;

        internal HalfTimeState(
            GameStateManager stateManager) : base(stateManager) {}

        internal override void Enter()
        {
            stateManager.OnHalfTimeStarted?.Invoke();
        }
    }

}
