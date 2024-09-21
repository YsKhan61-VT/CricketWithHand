﻿namespace DoozyPractice.Gameplay
{
    public class OtherBattingState : GameState
    {
        public override GameStateCategory StateCategory { get; } = GameStateCategory.Other_Batting;

        private GameplayUIMediator _gameplayUIMediator;

        public OtherBattingState(GameStateManager stateManager, GameplayUIMediator gameplayUIMediator)
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
            StateManager.OtherDidbat = true;
        }
    }

}
