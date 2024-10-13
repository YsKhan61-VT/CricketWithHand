using CricketWithHand.Gameplay;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ClientUI : MonoBehaviour
    {
        [SerializeField]
        private GameConfigSO _gameConfig;

        [SerializeField]
        private GameDataSO _gameData;

        [SerializeField]
        private PlayerStatsSO _playerStats;

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

        bool IsOwner => _playerStats.PlayerType == PlayerType.OWNER;

        bool IsBatting => (IsOwner && _gameData.IsOwnerBatting) ||
            (!IsOwner && !_gameData.IsOwnerBatting);

        private void OnEnable()
        {
            _gameData.TotalOversCountDataContainer.OnValueUpdated += UpdateOversUI;
            _gameData.TotalWicketsCountDataContainer.OnValueUpdated += UpdateWicketsUI;

            _playerStats.OversCount.OnValueUpdated += UpdateOversUI;
            _playerStats.BallsCount.OnValueUpdated += UpdateOversUI;
            _playerStats.TotalWicketsLost.OnValueUpdated += UpdateWicketsUI;
            _playerStats.InputScore.OnValueUpdated += UpdateInputScoreUI;
            _playerStats.TotalScore.OnValueUpdated += UpdateTotalScoreUI;
        }

        private void OnDisable()
        {
            _gameData.TotalOversCountDataContainer.OnValueUpdated -= UpdateOversUI;
            _gameData.TotalWicketsCountDataContainer.OnValueUpdated -= UpdateWicketsUI;

            _playerStats.OversCount.OnValueUpdated -= UpdateOversUI;
            _playerStats.BallsCount.OnValueUpdated -= UpdateOversUI;
            _playerStats.TotalWicketsLost.OnValueUpdated -= UpdateWicketsUI;
            _playerStats.InputScore.OnValueUpdated -= UpdateInputScoreUI;
            _playerStats.TotalScore.OnValueUpdated -= UpdateTotalScoreUI;
        }

        /// <summary>
        /// This should be called at the start of each half, 
        /// better from TurnController's OnNextHalfStarted event
        /// </summary>
        public void UpdatePlayingStateText() => _playingStateText.text = IsBatting ?
            _gameConfig.BattingPlayStateText : _gameConfig.BallingPlayStateText;

        private void UpdateInputScoreUI() =>
            _inputScoreText.text = _playerStats.InputScore.Value.ToString();

        private void UpdateTotalScoreUI() =>
            _totalScoreText.text = _playerStats.TotalScore.Value.ToString();

        private void UpdateWicketsUI() =>
            _totalWicketsText.text = $"{_playerStats.TotalWicketsLost.Value} / {_gameData.TotalWicketsCountDataContainer.Value}";

        /// <summary>
        /// Here the over count of the game will be 1, but in the score board we show the first over as 0,
        /// hence we subtract one from the actual over count. But, at start of game, when over count is zero for each client, as
        /// they didn't even start playing, that time we wanna show 0 to the UI also, not -1, hence we use Mathf.Max
        /// </summary>
        private void UpdateOversUI() =>
            _totalOversText.text = $"{Mathf.Max(0, _playerStats.OversCount.Value - 1)}.{_playerStats.BallsCount.Value} / {_gameData.TotalOversCountDataContainer.Value}";
    }
}
