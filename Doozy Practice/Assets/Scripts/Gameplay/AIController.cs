using UnityEngine;


namespace DoozyPractice.Gameplay
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        public void GiveRandomInput()
        {
            int score = Random.Range(1, 7);
            _turnController.RegisterOtherInput(score);
        }
    }
}
