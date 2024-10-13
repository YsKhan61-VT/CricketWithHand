using CricketWithHand.Gameplay;
using CricketWithHand.Utility;
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
        private PlayerType _playerType;

        [SerializeField]
        private TMP_Text _totalScoreText;

        [SerializeField]
        private TMP_Text _oversPlayedText;

        [SerializeField]
        private TMP_Text _wicketsLostText;


        bool IsOwner => _playerType == PlayerType.OWNER;
        int TotalScore => IsOwner ? _gameData.OwnerTotalScoreContainer.Value : _gameData.OtherTotalScoreContainer.Value;
        int OversPlayed => IsOwner ? _gameData.OwnerOverCountContainer.Value : _gameData.OtherOverCountContainer.Value;
        int BallsPlayed => IsOwner ? _gameData.OwnerBallCountContainer.Value : _gameData.OtherBallCountContainer.Value;
        int WicketsLost => IsOwner ? _gameData.OwnerTotalWicketLostContainer.Value : _gameData.OtherTotalWicketLostContainer.Value;

        public void UpdateTotalStats()
        {
            UpdateTotalScoreText();
            UpdateOversPlayedText();
            UpdateWicketsLostText();
        }

        private void UpdateTotalScoreText() =>
            _totalScoreText.text = TotalScore.ToString();

        /// <summary>
        /// Here the over count of the game will be 1, but in the score board we show the first over as 0,
        /// hence we subtract one from the actual over count. But, at start of game, when over count is zero for each client, as
        /// they didn't even start playing, that time we wanna show 0 to the UI also, not -1, hence we use Mathf.Max
        /// </summary>

        private void UpdateOversPlayedText()
        {
            int oversPlayed = Mathf.Max(0, OversPlayed - 1);
            string oversPlayedText = (BallsPlayed == 6 || BallsPlayed == 0) ?
                oversPlayed.ToString() : $"{oversPlayed}.{BallsPlayed}";
            _oversPlayedText.text = oversPlayedText;
        }

        private void UpdateWicketsLostText() =>
            _wicketsLostText.text = $"{WicketsLost}/{_gameData.TotalWicketsCountDataContainer.Value}";
    }
}
