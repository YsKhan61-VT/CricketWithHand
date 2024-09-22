using UnityEngine;


namespace CricketWithHand.Gameplay
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        [SerializeField]
        TossController _tossController;

        [SerializeField, Range(0f, 1f)]
        float _aiBattingChance;


        public void GiveRandomInput()
        {
            int score = Random.Range(1, 7);
            _turnController.RegisterOtherInput(score);
        }

        public void ChooseBatOrBall()
        {
            bool willBat = Random.Range(0f, 1f) <= _aiBattingChance;
            _tossController.ChooseBatOrBallFirst(false, willBat);
        }
    }
}
