using CricketWithHand.Gameplay;
using System.Collections.Generic;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class GameStatsUI : MonoBehaviour
    {
        [SerializeField]
        private PlayerStatsSO _playerStats;

        [SerializeField]
        private OverStatsUI _overStatsUIPrefab;

        [SerializeField]
        private Transform _overStatsSpawnParent;

        private List<OverStatsUI> _overStatsUIs = new();

        private void OnEnable()
        {
            _playerStats.OversCount.OnValueUpdated += OnOverCountUpdated;
            _playerStats.OnBallPlayed += OnBallPlayed;
        }

        private void OnDisable()
        {
            _playerStats.OversCount.OnValueUpdated -= OnOverCountUpdated;
            _playerStats.OnBallPlayed -= OnBallPlayed;
        }

        private void OnOverCountUpdated()
        {
            if (_playerStats.OversCount.Value > 0 &&
                _overStatsUIs != null &&
                _overStatsUIs.Count == _playerStats.OversCount.Value - 1)
            {
                OverStatsUI overStatsUI = SpawnOverStatsUI();
                _overStatsUIs.Add(overStatsUI);
                overStatsUI.UpdateOverCountText(_playerStats.OversCount.Value);
            }
        }

        /// <summary>
        /// Here the over count of the game will be 1, but in the score board we show the first over as 0,
        /// hence we subtract one from the actual over count. But, at start of game, when over count is zero for each client, as
        /// they didn't even start playing, that time we wanna show 0 to the UI also, not -1, hence we use Mathf.Max
        /// </summary>
        private void OnBallPlayed()
        {
            if (_playerStats.OversCount.Value <= 0) return;                           // This happens the first time when the game starts, the over count = 0, hence _overStatsUI[index] = null as OnOverCountUpdated is not yet called that time.

            int index = Mathf.Max(0, (_playerStats.OversCount.Value - 1));

            _overStatsUIs[index].UpdateScoreInBallText(
                _playerStats.BallsCount.Value, _playerStats.ScoreInThisBall.Value, _playerStats.IsOutInThisBall.Value);
        }

        private OverStatsUI SpawnOverStatsUI() =>
            Instantiate(_overStatsUIPrefab, _overStatsSpawnParent);
    }
}
