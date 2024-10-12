using CricketWithHand.Gameplay;
using CricketWithHand.Utility;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ClientUI : MonoBehaviour
    {
        [SerializeField]
        IntDataContainerSO _totalOvers;

        [SerializeField]
        IntDataContainerSO _totalWickets;

        [SerializeField]
        BoolDataContainerSO _hasStartedBatting;

        [SerializeField]
        IntDataContainerSO _overCount;

        [SerializeField]
        IntDataContainerSO _ballCount;

        [SerializeField]
        IntDataContainerSO _inputScore;

        [SerializeField]
        IntDataContainerSO _totalScore;

        [SerializeField]
        IntDataContainerSO _wicketsLost;

        [SerializeField]
        TMP_Text _playingStateText;

        [SerializeField]
        TMP_Text _inputScoreText;

        [SerializeField]
        TMP_Text _totalScoreText;

        [SerializeField]
        TMP_Text _totalWicketsText;

        [SerializeField]
        TMP_Text _totalOversText;

        [SerializeField]
        string _battingText = "Batting";

        [SerializeField]
        string _ballingText = "Balling";

        private void OnEnable()
        {
            _totalOvers.OnValueUpdated += UpdateOversUI;
            _overCount.OnValueUpdated += UpdateOversUI;
            _ballCount.OnValueUpdated += UpdateOversUI;

            _totalWickets.OnValueUpdated += UpdateWicketsUI;
            _wicketsLost.OnValueUpdated += UpdateWicketsUI;

            _hasStartedBatting.OnValueUpdated += UpdatePlayingStateText;
            _inputScore.OnValueUpdated += UpdateInputScoreUI;
            _totalScore.OnValueUpdated += UpdateTotalScoreUI;
        }

        private void OnDisable()
        {
            _totalOvers.OnValueUpdated -= UpdateOversUI;
            _overCount.OnValueUpdated -= UpdateOversUI;
            _ballCount.OnValueUpdated -= UpdateOversUI;

            _totalWickets.OnValueUpdated -= UpdateWicketsUI;
            _wicketsLost.OnValueUpdated -= UpdateWicketsUI;

            _hasStartedBatting.OnValueUpdated -= UpdatePlayingStateText;
            _inputScore.OnValueUpdated -= UpdateInputScoreUI;
            _totalScore.OnValueUpdated -= UpdateTotalScoreUI;
        }

        private void UpdatePlayingStateText() =>
            _playingStateText.text = _hasStartedBatting.Value ? 
            _battingText : _ballingText;

        private void UpdateInputScoreUI() =>
            _inputScoreText.text = _inputScore.Value.ToString();

        private void UpdateTotalScoreUI() =>
            _totalScoreText.text = _totalScore.Value.ToString();

        private void UpdateWicketsUI() =>
            _totalWicketsText.text = $"{_wicketsLost.Value} / {_totalWickets.Value}";

        private void UpdateOversUI() =>
            _totalOversText.text = $"{_overCount.Value}.{_ballCount.Value} / {_totalOvers.Value.ToString()}";
    }
}
