using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class OverStatsUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text _overCountText;

        [SerializeField]
        TMP_Text[] _ScoreInBallTexts;

        public void UpdateOverCountText(int number) =>
            _overCountText.text = number.ToString();

        public void UpdateScoreInBallText(int ballNumber, int score, bool isOut)
        {
            if (ballNumber <= 0 || ballNumber > 6)
                return;             // This can happen at start of each half, when over count = 0, ball count = 0

            _ScoreInBallTexts[ballNumber - 1].text = isOut ? "W" : score.ToString();
        }
    }
}

