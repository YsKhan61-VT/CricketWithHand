using CricketWithHand.Utility;
using UnityEngine;


namespace CricketWithHand.Gameplay
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        private GameConfigSO _gameConfig;

        [SerializeField]
        private TurnController _turnController;

        [SerializeField]
        private TossController _tossController;


        public void GiveRandomInput()
        {
            int score = Random.Range(1, 7);
            _turnController.RegisterOtherInput(score);
        }

        public void ChooseBatOrBall()
        {
            bool willBat = Random.Range(0f, 1f) <= _gameConfig.AIBattingChance;
            _tossController.ChooseBatOrBallFirst(false, willBat);
        }
    }
}
