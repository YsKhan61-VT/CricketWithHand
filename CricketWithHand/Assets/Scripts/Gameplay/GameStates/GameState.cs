namespace CricketWithHand.Gameplay
{
    public enum GameStateCategory
    {
        Owner_Batting,
        FirstHalf,
        HalfTime,
        SecondHalf,
        Other_Batting,
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
