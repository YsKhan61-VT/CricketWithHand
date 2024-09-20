using UnityEngine;
using UnityEngine.Events;


namespace DoozyPractice.UI
{
    public class InputPanelButtonUI : MonoBehaviour
    {
        [SerializeField]
        UnityEvent<bool> _onToggleActiveInputButtons;

        public void ToggleActiveInputButtons(bool toggle) =>
            _onToggleActiveInputButtons?.Invoke(toggle);
    }
}
