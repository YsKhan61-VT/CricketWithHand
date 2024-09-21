namespace DoozyPractice.Gameplay
{
    public abstract class GameState
    {
        public abstract GameStateCategory StateCategory { get; }
        public GameStateManager StateManager { get; protected set; }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }

}
