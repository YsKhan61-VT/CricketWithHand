namespace DoozyPractice.Gameplay
{
    public abstract class GameState
    {
        public abstract GameStateCategory StateCategory { get; }
        public GameStateManager StateManager { get; private set; }

        public GameState(GameStateManager stateManager)
        {
            StateManager = stateManager;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }

}
