using UnityEngine;


namespace DoozyPractice.Gameplay
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        [SerializeField]
        TossController _tossController;

        public void GiveRandomInput()
        {
            int score = Random.Range(1, 7);
            _turnController.RegisterOtherInput(score);
        }

        public void ChooseBatOrBall()
        {
            bool willBat = Random.Range(0f, 1f) > 0.5f;
            _tossController.ChooseBatOrBallFirst(false, willBat);
        }
    }
}
