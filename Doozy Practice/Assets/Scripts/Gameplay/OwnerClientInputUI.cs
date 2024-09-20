using DoozyPractice.Gameplay;
using UnityEngine;


namespace DoozyPractice.UI
{


    public class OwnerClientInputUI : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        public void OnScoreOneButtonClicked() =>
            _turnController.OwnerGaveInput((int)InputScore.One);

        public void OnScoreTwoButtonClicked() =>
            _turnController.OwnerGaveInput((int)InputScore.Two);

        public void OnScoreThreeButtonClicked() =>
            _turnController.OwnerGaveInput((int)InputScore.Three);

        public void OnScoreFourButtonClicked() =>
            _turnController.OwnerGaveInput((int)InputScore.Four);

        public void OnScoreFiveButtonClicked() =>
            _turnController.OwnerGaveInput((int)InputScore.Five);

        public void OnScoreSixButtonClicked() =>
            _turnController.OwnerGaveInput((int)InputScore.Six);
    }
}
