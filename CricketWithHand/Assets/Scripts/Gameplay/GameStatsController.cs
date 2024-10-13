using CricketWithHand.UI;
using CricketWithHand.Utility;
using System.Collections.Generic;
using UnityEngine;


namespace CricketWithHand.Gameplay
{
    public class GameStatsController : MonoBehaviour
    {
        /// <summary>
        /// Called everytime owner is playing a new over
        /// </summary>
        [SerializeField]
        IntDataContainerSO _ownerOverCount;

        /// <summary>
        /// Called everytime other player is playing a new over
        /// </summary>
        [SerializeField]
        IntDataContainerSO _otherOverCount;

        /// <summary>
        /// Called everytime owner played a new ball
        /// </summary>
        [SerializeField]
        BallDataContainerSO _ownerBallDataContainer;

        /// <summary>
        /// Called everytime other played a new ball
        /// </summary>
        [SerializeField]
        BallDataContainerSO _otherBallDataContainer;

        [SerializeField]
        OverStatsUI _overStatsUIPrefab;

        [SerializeField]
        Transform _ownerOverStatsSpawnParent;

        [SerializeField]
        Transform _otherOverStatsSpawnParent;

        private List<OverStatsUI> _ownerOverStatsUIs = new();
        private List<OverStatsUI> _otherOverStatsUIs = new();

        private void OnEnable()
        {
            _ownerOverCount.OnValueUpdated += OnOwnerOverCountUpdated;
            _otherOverCount.OnValueUpdated += OnOtherOverCountUpdated;

            _ownerBallDataContainer.OnValueUpdated += OnOwnerBallDataUpdated;
            _otherBallDataContainer.OnValueUpdated += OnOtherBallDataUpdated;
        }

        private void OnDisable()
        {
            _ownerOverCount.OnValueUpdated -= OnOwnerOverCountUpdated;
            _otherOverCount.OnValueUpdated -= OnOtherOverCountUpdated;

            _ownerBallDataContainer.OnValueUpdated -= OnOwnerBallDataUpdated;
            _otherBallDataContainer.OnValueUpdated -= OnOtherBallDataUpdated;
        }

        void OnOwnerOverCountUpdated()
        {
            if (_ownerOverCount.Value > 0 && 
                _ownerOverStatsUIs != null && 
                _ownerOverStatsUIs.Count == _ownerOverCount.Value - 1)
            {
                OverStatsUI overStatsUI = SpawnOverStatsUI(true);
                _ownerOverStatsUIs.Add(overStatsUI);
                overStatsUI.UpdateOverCountText(_ownerOverCount.Value);
            }
        }

        void OnOtherOverCountUpdated()
        {
            if (_otherOverCount.Value > 0 &&
                _otherOverStatsUIs != null &&
                _otherOverStatsUIs.Count ==_otherOverCount.Value - 1)
            {
                OverStatsUI overStatsUI = SpawnOverStatsUI(false);
                _otherOverStatsUIs.Add(overStatsUI);
                overStatsUI.UpdateOverCountText(_otherOverCount.Value);
            }
        }

        /// <summary>
        /// Here the over count of the game will be 1, but in the score board we show the first over as 0,
        /// hence we subtract one from the actual over count. But, at start of game, when over count is zero for each client, as
        /// they didn't even start playing, that time we wanna show 0 to the UI also, not -1, hence we use Mathf.Max
        /// </summary>
        private void OnOwnerBallDataUpdated()
        {
            BallData ballData = _ownerBallDataContainer.Value;
            if (ballData.OverNumber <= 0) return;
            _ownerOverStatsUIs[Mathf.Max(0, ballData.OverNumber - 1)].UpdateScoreInBallText(
                ballData.BallNumber, ballData.Score, ballData.IsWicketLost);
        }

        /// <summary>
        /// Here the over count of the game will be 1, but in the score board we show the first over as 0,
        /// hence we subtract one from the actual over count. But, at start of game, when over count is zero for each client, as
        /// they didn't even start playing, that time we wanna show 0 to the UI also, not -1, hence we use Mathf.Max
        /// </summary>
        private void OnOtherBallDataUpdated()
        {
            BallData ballData = _otherBallDataContainer.Value;
            if (ballData.OverNumber <= 0) return;
            _otherOverStatsUIs[Mathf.Max(0, ballData.OverNumber - 1)].UpdateScoreInBallText(
                ballData.BallNumber, ballData.Score, ballData.IsWicketLost);
        }

        OverStatsUI SpawnOverStatsUI(bool isOwner)
        {
            if (isOwner)
                return Instantiate(_overStatsUIPrefab, _ownerOverStatsSpawnParent);
            else
                return Instantiate(_overStatsUIPrefab, _otherOverStatsSpawnParent);
        }
    }
}

