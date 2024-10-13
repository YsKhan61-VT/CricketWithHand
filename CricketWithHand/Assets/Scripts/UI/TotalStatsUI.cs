using CricketWithHand.Gameplay;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    /// <summary>
    /// This script displays the total stats on the GameStatsUI during half time, and full time
    /// </summary>
    public class TotalStatsUI : MonoBehaviour
    {
        [SerializeField]
        private GameDataSO _gameData;

        [SerializeField]
        private PlayerStatsSO _playerStats;

        [SerializeField]
        private TMP_Text _totalScoreText;

        [SerializeField]
        private TMP_Text _oversPlayedText;

        [SerializeField]
        private TMP_Text _wicketsLostText;


        bool IsOwner => _playerStats.PlayerType == PlayerType.OWNER;

        public void UpdateTotalStats()
        {
            UpdateTotalScoreText();
            UpdateOversPlayedText();
            UpdateWicketsLostText();
        }

        private void UpdateTotalScoreText() =>
            _totalScoreText.text = _playerStats.TotalScore.Value.ToString();

        /// <summary>
        /// Here the over count of the game will be 1, but in the score board we show the first over as 0,
        /// hence we subtract one from the actual over count. But, at start of game, when over count is zero for each client, as
        /// they didn't even start playing, that time we wanna show 0 to the UI also, not -1, hence we use Mathf.Max
        /// </summary>

        private void UpdateOversPlayedText()
        {
            int ballsCount = _playerStats.BallsCount.Value;
            int oversPlayed = Mathf.Max(0, _playerStats.OversCount.Value - 1);

            string oversPlayedText = (ballsCount == 6 || ballsCount == 0) ?
                oversPlayed.ToString() : $"{oversPlayed}.{ballsCount}";

            _oversPlayedText.text = oversPlayedText;
        }

        private void UpdateWicketsLostText() =>
            _wicketsLostText.text = $"{_playerStats.TotalWicketsLost.Value}/{_gameData.TotalWicketsCountDataContainer.Value}";
    }
}
