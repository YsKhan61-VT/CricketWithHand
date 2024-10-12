using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using CricketWithHand.Utility;


namespace CricketWithHand.Gameplay
{
    public class TurnSliderUI : MonoBehaviour
    {
        [SerializeField]
        FloatDataContainerSO _turnSliderValue;

        [SerializeField]
        UISlider _turnDurationSlider;

        private void OnEnable() =>
            _turnSliderValue.OnValueUpdated += UpdateTurnDurationSlider;

        private void OnDisable() =>
            _turnSliderValue.OnValueUpdated -= UpdateTurnDurationSlider;

        private void UpdateTurnDurationSlider() =>
            _turnDurationSlider.value = _turnSliderValue.Value;
    }
}
