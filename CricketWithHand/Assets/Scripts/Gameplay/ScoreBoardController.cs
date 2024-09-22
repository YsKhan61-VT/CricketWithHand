using CricketWithHand.UI;
using CricketWithHand.Utility;
using System.Collections.Generic;
using UnityEngine;


namespace CricketWithHand.Gameplay
{
    public class ScoreBoardController : MonoBehaviour
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

        [SerializeField]
        GameStatsSO _ownerStats;

        [SerializeField]
        GameStatsSO _otherStats;

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
        }

        private void OnDisable()
        {
            _ownerOverCount.OnValueUpdated -= OnOwnerOverCountUpdated;
            _otherOverCount.OnValueUpdated -= OnOtherOverCountUpdated;
        }

        void OnOwnerOverCountUpdated()
        {
            if (_ownerOverCount.Value > 0 && 
                _ownerOverStatsUIs != null && 
                _ownerOverStatsUIs.Count == _ownerOverCount.Value - 1)
            {
                SpawnOverStatsUI(true);
            }
        }

        void OnOtherOverCountUpdated()
        {
            if (_otherOverCount.Value > 0 &&
                _otherOverStatsUIs != null &&
                _otherOverStatsUIs.Count ==_ownerOverCount.Value - 1)
            {
                SpawnOverStatsUI(false);
            }
        }

        void SpawnOverStatsUI(bool isOwner)
        {
            if (isOwner)
            {
                _ownerOverStatsUIs.Add(Instantiate(_overStatsUIPrefab, _ownerOverStatsSpawnParent));
            }
            else
            {
                _otherOverStatsUIs.Add(Instantiate(_overStatsUIPrefab, _otherOverStatsSpawnParent));
            }
        }
    }
}

