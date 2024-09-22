namespace CricketWithHand.Gameplay
{
    public class HalfTimeState : GameState
    {
        public override GameStateCategory StateCategory => GameStateCategory.HalfTime;

        public HalfTimeState(
            GameStateManager stateManager) : base(stateManager)
        {
        }

        public override void Enter()
        {
            StateManager.OnHalfTimeStarted?.Invoke();
        }
    }

}
