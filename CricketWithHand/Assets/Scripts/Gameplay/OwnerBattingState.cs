namespace CricketWithHand.Gameplay
{
    public class OwnerBattingState : GameState
    {
        public override GameStateCategory StateCategory { get; } = GameStateCategory.Owner_Batting;
        
        private GameplayUIMediator _gameplayUIMediator;

        public OwnerBattingState(
            GameStateManager stateManager, 
            GameplayUIMediator gameplayUIMediator) : base(stateManager)
        {
            _gameplayUIMediator = gameplayUIMediator;
        }

        public override void Enter()
        {
            StateManager.OwnerStartedBat = true;
            StateManager.OnOwnerBattingStateStarted?.Invoke();
        }
    }

}
