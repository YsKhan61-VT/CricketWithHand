namespace CricketWithHand.Gameplay
{
    public class OtherBattingState : GameState
    {
        internal override GameStateCategory StateCategory { get; } = GameStateCategory.Other_Batting;

        private GameplayUIMediator _gameplayUIMediator;

        internal OtherBattingState(
            GameStateManager stateManager, 
            GameplayUIMediator gameplayUIMediator) : base(stateManager)
        {
            _gameplayUIMediator = gameplayUIMediator;
        }

        internal override void Enter()
        {
            StateManager.OtherStartedBat = true;
            StateManager.OnOtherBattingStateStarted?.Invoke();
        }
    }

}
