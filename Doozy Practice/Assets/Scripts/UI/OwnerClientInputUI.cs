using DoozyPractice.Gameplay;
using UnityEngine;
using UnityEngine.Events;


namespace DoozyPractice.UI
{

    public class OwnerClientInputUI : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        [SerializeField]
        UnityEvent<bool> _onToggleActiveInputButtons;

        public void ToggleActiveInputButtons(bool toggle) =>
            _onToggleActiveInputButtons?.Invoke(toggle);

        public void OnScoreOneButtonClicked() =>
            _turnController.RegisterOwnerInput((int)InputScore.One);

        public void OnScoreTwoButtonClicked() =>
            _turnController.RegisterOwnerInput((int)InputScore.Two);

        public void OnScoreThreeButtonClicked() =>
            _turnController.RegisterOwnerInput((int)InputScore.Three);

        public void OnScoreFourButtonClicked() =>
            _turnController.RegisterOwnerInput((int)InputScore.Four);

        public void OnScoreFiveButtonClicked() =>
            _turnController.RegisterOwnerInput((int)InputScore.Five);

        public void OnScoreSixButtonClicked() =>
            _turnController.RegisterOwnerInput((int)InputScore.Six);
    }
}
