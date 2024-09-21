namespace DoozyPractice.Gameplay
{
    public class OwnerBattingState : GameState
    {
        public override GameStateCategory StateCategory { get; } = GameStateCategory.Owner_Batting;
        
        private GameplayUIMediator _gameplayUIMediator;

        public OwnerBattingState(GameStateManager stateManager, GameplayUIMediator gameplayUIMediator)
        {
            StateManager = stateManager;
            _gameplayUIMediator = gameplayUIMediator;
        }

        public override void Enter()
        {
            StateManager.OnOtherBattingStateStarted?.Invoke();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            StateManager.OwnerDidBat = true;
        }
    }

}
