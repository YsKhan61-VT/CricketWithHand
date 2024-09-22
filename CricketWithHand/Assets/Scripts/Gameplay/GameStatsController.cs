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

        /*[SerializeField]
        GameStatsSO _ownerStats;

        [SerializeField]
        GameStatsSO _otherStats;*/

        [SerializeField]
        OverStatsUI _overStatsUIPrefab;

        [SerializeField]
        Transform _ownerOverStatsSpawnParent;

        [SerializeField]
        Transform _otherOverStatsSpawnParent;

        private List<OverStatsUI> _ownerOverStatsUIs = new();
        private List<OverStatsUI> _otherOverStatsUIs = new();

        private int _currentOwnerOverStatsUIIndex = -1;
        private int _currentOtherOverStatsUIIndex = -1;

        private void OnEnable()
        {
            _ownerOverCount.OnValueUpdated += OnOwnerOverCountUpdated;
            _otherOverCount.OnValueUpdated += OnOtherOverCountUpdated;

            _ownerBallDataContainer.OnValueUpdated += OnOwnerBallDataUpdated;
            _otherBallDataContainer.OnValueUpdated += OnOtherBallDataUpdated;

            // _ownerStats.SubscribeToEvents();
            // _otherStats.SubscribeToEvents();
        }

        private void OnDisable()
        {
            _ownerOverCount.OnValueUpdated -= OnOwnerOverCountUpdated;
            _otherOverCount.OnValueUpdated -= OnOtherOverCountUpdated;

            _ownerBallDataContainer.OnValueUpdated -= OnOwnerBallDataUpdated;
            _otherBallDataContainer.OnValueUpdated -= OnOtherBallDataUpdated;

            // _ownerStats.UnsubscribeFromEvents();
            // _otherStats.UnsubscribeFromEvents();
        }

        void OnOwnerOverCountUpdated()
        {
            if (_ownerOverCount.Value > 0 && 
                _ownerOverStatsUIs != null && 
                _ownerOverStatsUIs.Count == _ownerOverCount.Value - 1)
            {
                OverStatsUI overStatsUI = SpawnOverStatsUI(true);
                _ownerOverStatsUIs.Add(overStatsUI);
                _currentOwnerOverStatsUIIndex++;
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
                _currentOtherOverStatsUIIndex++;
                overStatsUI.UpdateOverCountText(_otherOverCount.Value);
            }
        }

        private void OnOwnerBallDataUpdated()
        {
            BallData ballData = _ownerBallDataContainer.Value;
            _ownerOverStatsUIs[_currentOwnerOverStatsUIIndex].UpdateScoreInBallText(
                ballData.BallNumber, ballData.Score, ballData.IsWicketLost);
        }

        private void OnOtherBallDataUpdated()
        {
            BallData ballData = _otherBallDataContainer.Value;
            _otherOverStatsUIs[_currentOtherOverStatsUIIndex].UpdateScoreInBallText(
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

