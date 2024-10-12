namespace CricketWithHand.Gameplay
{
    public class FirstHalfState : GameState
    {

        private TurnController _turnController;
        internal override GameStateCategory StateCategory => GameStateCategory.FirstHalf;

        internal FirstHalfState(GameStateManager stateManager, TurnController turnController) : base(stateManager)
        {
            _turnController = turnController;
        }

        internal override void Enter()
        {
            _turnController.StartHalf();
        }
    }

}
