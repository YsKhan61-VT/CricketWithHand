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
            StateManager.OwnerDidBat = true;
            _gameplayUIMediator.UpdateUIToOwnerIsBatting();
            _gameplayUIMediator.UpdateUIToOtherIsBalling();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }

}
