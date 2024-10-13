using CricketWithHand.Utility;
using System;
using UnityEngine;


namespace CricketWithHand.Gameplay
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/DataContainers/PlayerStatsSO")]
    public class PlayerStatsSO : ScriptableObject
    {
        #region Events

        public Action OnBallPlayed;

        #endregion

        #region Datas

        /// <summary>
        /// Just to mark this asset as the respective player
        /// </summary>
        public PlayerType PlayerType;

        /// <summary>
        /// Player started to bat
        /// </summary>
        public BoolDataContainerSO StartedBatting;

        /// <summary>
        /// Player's input score
        /// </summary>
        public IntDataContainerSO InputScore;

        /// <summary>
        /// Over number player is playing
        /// </summary>
        public IntDataContainerSO OversCount;

        /// <summary>
        /// Ball number of this over, player is playing
        /// </summary>
        public IntDataContainerSO BallsCount;

        /// <summary>
        /// Score made by player in this ball, if batting
        /// </summary>
        public IntDataContainerSO ScoreInThisBall;

        /// <summary>
        /// Is the player out in this ball
        /// </summary>
        public BoolDataContainerSO IsOutInThisBall;

        /// <summary>
        /// Total score of the player
        /// </summary>
        public IntDataContainerSO TotalScore;

        /// <summary>
        /// Total wickets lost by the player
        /// </summary>
        public IntDataContainerSO TotalWicketsLost;

        #endregion

        public void ResetData()
        {
            StartedBatting.UpdateData(false);
            InputScore.UpdateData(0);
            OversCount.UpdateData(0);
            BallsCount.UpdateData(0);
            ScoreInThisBall.UpdateData(0);
            IsOutInThisBall.UpdateData(false);
            TotalScore.UpdateData(0);
            TotalWicketsLost.UpdateData(0);
        }
    }
}
