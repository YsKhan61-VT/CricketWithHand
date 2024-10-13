using System.Text;
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
    public class BallDataContainerSO : GenericDataContainerSO<BallData> 
    {
        protected override void Log()
        {
            // Remove 'using' since StringBuilder doesn't require disposal
            StringBuilder builder = new();

            // Append each log line
            builder.Append($"OverNumber: {Value.OverNumber} \n");
            builder.Append($"BallNumber: {Value.BallNumber} \n");
            builder.Append($"Score: {Value.Score} \n");
            builder.Append($"IsWicketLost: {Value.IsWicketLost}");

            _logArea = builder.ToString();
        }
    }
}

