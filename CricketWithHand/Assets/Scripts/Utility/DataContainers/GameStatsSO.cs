using System;
using System.Collections.Generic;
using UnityEngine;

namespace CricketWithHand.Utility
{
    public class Over
    {
        public int[] Scores =new int[6];
    }

    [CreateAssetMenu(fileName = "Scoreboard Stats", menuName = "ScriptableObjects/DataContainers/ScoreboardStats")]
    public class GameStatsSO : ScriptableObject
    {
        public Action OnNewOverStorageCreated;

        public List<Over> Overs;

        public void CreateNewOverStorage()
        {
            
        }
        
    }
}

