using TMPro;
using UnityEngine;


namespace DoozyPractice.UI
{
    public class ClientUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text _playingStateText;

        [SerializeField]
        TMP_Text _inputScoreText;

        [SerializeField]
        TMP_Text _totalScoreText;

        [SerializeField]
        TMP_Text _totalOversText;

        [SerializeField]
        string _battingText = "Batting";

        [SerializeField]
        string _ballingText = "Balling";

        public void UpdatePlayingStateText(bool isBatting)
        {
            if (isBatting)
            {
                _playingStateText.text = _battingText;
            }
            else
            {
                _playingStateText.text = _ballingText;
            }
        }

        public void ShowInputScore(int score) =>
            _inputScoreText.text = score.ToString();

        public void UpdateTotalScoreAndWicket(int score, int wicket) =>
            _totalScoreText.text = $"{score} / {wicket}";

        public void UpdateTotalOversText(int over, int balls, int totalOvers)
        {
            string totalOversText = totalOvers != -1 ? totalOvers.ToString() : "-";
            _totalOversText.text = $"{over}.{balls} / {totalOversText}";
        }
    }
}
