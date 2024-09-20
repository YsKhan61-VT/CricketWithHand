namespace DoozyPractice.Gameplay
{
    public abstract class GameState
    {
        public abstract GameStateCategory StateCategory { get; }
        public GameStateManager StateManager { get; protected set; }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }

}
