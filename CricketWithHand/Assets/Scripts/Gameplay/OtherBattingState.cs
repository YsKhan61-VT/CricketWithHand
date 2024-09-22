namespace DoozyPractice.Gameplay
{
    public class OtherBattingState : GameState
    {
        public override GameStateCategory StateCategory { get; } = GameStateCategory.Other_Batting;

        private GameplayUIMediator _gameplayUIMediator;

        public OtherBattingState(
            GameStateManager stateManager, 
            GameplayUIMediator gameplayUIMediator) : base(stateManager)
        {
            _gameplayUIMediator = gameplayUIMediator;
        }

        public override void Enter()
        {
            StateManager.OtherDidBatting = true;
            StateManager.OnOtherBattingStateStarted?.Invoke();
        }
    }

}
