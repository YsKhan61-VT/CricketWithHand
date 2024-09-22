using UnityEngine;

namespace CricketWithHand.Utility
{
    public struct BallData
    {
        public int OverNumber;
        public int BallNumber;
        public int Score;
        public bool IsWicketLost;
    }

    [CreateAssetMenu(fileName = "BallDataContainer", menuName = "ScriptableObjects/DataContainers/BallDataContainer")]
    public class BallDataContainerSO : GenericDataContainerSO<BallData> { }
}

