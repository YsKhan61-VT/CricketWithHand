namespace CricketWithHand.Gameplay
{
    public class OwnerBattingState : GameState
    {
        internal override GameStateCategory StateCategory { get; } = GameStateCategory.Owner_Batting;
        
        private GameplayUIMediator _gameplayUIMediator;

        internal OwnerBattingState(
            GameStateManager stateManager, 
            GameplayUIMediator gameplayUIMediator) : base(stateManager)
        {
            _gameplayUIMediator = gameplayUIMediator;
        }

        internal override void Enter()
        {
            StateManager.OwnerStartedBat = true;
            StateManager.OnOwnerBattingStateStarted?.Invoke();
        }
    }

}
