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

        public void OnScoreInputButtonClicked(int value) =>
            _turnController.RegisterOwnerInput(value);
    }
}
