namespace CricketWithHand.Gameplay
{
    public enum GameStateCategory
    {
        FirstHalf,
        HalfTime,
        SecondHalf,
        GameEnd
    }

    public abstract class GameState
    {
        internal abstract GameStateCategory StateCategory { get; }
        protected GameStateManager StateManager { get; private set; }

        internal GameState(GameStateManager stateManager)
        {
            StateManager = stateManager;
        }

        internal virtual void Enter() { }
        internal virtual void Update() { }
        internal virtual void Exit() { }
    }

}
