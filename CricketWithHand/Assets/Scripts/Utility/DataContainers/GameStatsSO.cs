using System;
using System.Collections.Generic;
using UnityEngine;

namespace CricketWithHand.Utility
{
    public class Over
    {
        public BallData[] Balls =new BallData[6];
    }

    // [CreateAssetMenu(fileName = "Scoreboard Stats", menuName = "ScriptableObjects/DataContainers/ScoreboardStats")]
    public class GameStatsSO : ScriptableObject
    {
        [SerializeField]
        IntDataContainerSO _overCountDataContainer;

        [SerializeField]

        public int LastOverNumber;
        public int LastBallNumber;

        public Action OnNewOverStorageCreated;

        public List<Over> Overs => new List<Over>();

        public void SubscribeToEvents()
        {
            _overCountDataContainer.OnValueUpdated += OnOverCountUpdated;
        }

        public void UnsubscribeFromEvents()
        {
            _overCountDataContainer.OnValueUpdated -= OnOverCountUpdated;
        }

        public void AddNewOver() => Overs.Add(new Over());

        public void CreateNewOverStorage()
        {
            
        }

        void OnOverCountUpdated()
        {
            AddNewOver();
        }

    }
}

