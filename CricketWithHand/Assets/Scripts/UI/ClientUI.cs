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

            _inputScore.OnValueUpdated -= UpdateInputScoreUI;
            _totalScore.OnValueUpdated -= UpdateTotalScoreUI;
        }

        public void UpdatePlayingStateText() =>
            _playingStateText.text = _hasStartedBatting.Value ? 
            _battingText : _ballingText;

        private void UpdateInputScoreUI() =>
            _inputScoreText.text = _inputScore.Value.ToString();

        private void UpdateTotalScoreUI() =>
            _totalScoreText.text = _totalScore.Value.ToString();

        private void UpdateWicketsUI() =>
            _totalWicketsText.text = $"{_wicketsLost.Value} / {_totalWickets.Value}";

        /// <summary>
        /// Here the over count of the game will be 1, but in the score board we show the first over as 0,
        /// hence we subtract one from the actual over count. But, at start of game, when over count is zero for each client, as
        /// they didn't even start playing, that time we wanna show 0 to the UI also, not -1, hence we use Mathf.Max
        /// </summary>
        private void UpdateOversUI() =>
            _totalOversText.text = $"{Mathf.Max(0, _overCount.Value - 1)}.{_ballCount.Value} / {_totalOvers.Value.ToString()}";
    }
}
