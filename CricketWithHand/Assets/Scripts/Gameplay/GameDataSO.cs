using CricketWithHand.Utility;
using UnityEngine;


namespace CricketWithHand.Gameplay
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/DataContainers/GameDataSO")]
    public class GameDataSO : ScriptableObject
    {
        public GameConfigSO GameConfig;

        public GameStateCategoryDataContainerSO CurrentGameStateCategory;

        public PlayerTypeDataContainerSO BatsmanOfFirstHalf;

        public IntDataContainerSO TotalOversCountDataContainer;

        public IntDataContainerSO TotalWicketsCountDataContainer;

        public FloatDataContainerSO TurnSliderValue;

        public PlayerTypeDataContainerSO Winner;


        [Space(10)]

        [Header("Owner Player Datas")]

        #region Owner Data Containers

        public BoolDataContainerSO HasOwnerBattingStarted;

        /// <summary>
        /// Invoked everytime owner gives input
        /// </summary>
        public IntDataContainerSO OwnerInputScoreContainer;

        /// <summary>
        /// Called everytime owner is playing a new over
        /// </summary>
        public IntDataContainerSO OwnerOverCountContainer;

        /// <summary>
        /// Called everytime owner is playing a new ball
        /// </summary>
        public IntDataContainerSO OwnerBallCountContainer;

        public IntDataContainerSO OwnerTurnScoreContainer;

        public BoolDataContainerSO OwnerIsOutOnThisTurnContainer;

        /// <summary>
        /// Called everytime owner played a new ball
        /// </summary>
        public BallDataContainerSO OwnerBallDataContainer;

        /// <summary>
        /// Called everytime the owner total score gets updated.
        /// </summary>
        public IntDataContainerSO OwnerTotalScoreContainer;

        public IntDataContainerSO OwnerTotalWicketLostContainer;

        #endregion

        [Space(5)]

        [Header("Other Player Datas")]

        #region Other Data Containers

        public BoolDataContainerSO HasOtherBattingStarted;

        /// <summary>
        /// Invoked everytime owner gives input
        /// </summary>
        public IntDataContainerSO OtherInputScoreContainer;

        /// <summary>
        /// Called everytime other player is playing a new over
        /// </summary>
        public IntDataContainerSO OtherOverCountContainer;

        /// <summary>
        /// Called everytime other player is playing a new ball
        /// </summary>
        public IntDataContainerSO OtherBallCountContainer;

        public IntDataContainerSO OtherTurnScoreContainer;

        public BoolDataContainerSO OtherIsOutOnThisTurnContainer;

        /// <summary>
        /// Called everytime other played a new ball
        /// </summary>
        public BallDataContainerSO OtherBallDataContainer;

        /// <summary>
        /// Called everytime the other player's total score gets updated.
        /// </summary>
        public IntDataContainerSO OtherTotalScoreContainer;

        public IntDataContainerSO OtherTotalWicketLostContainer;

        #endregion

        public void ResetRuntimeDatas()
        {
            TurnSliderValue.UpdateData(1);

            HasOwnerBattingStarted.UpdateData(false);
            OwnerInputScoreContainer.UpdateData(0);
            OwnerOverCountContainer.UpdateData(0);
            OwnerBallCountContainer.UpdateData(0);
            OwnerTurnScoreContainer.UpdateData(0);
            OwnerBallDataContainer.UpdateData(new BallData
            {
                OverNumber = 0,
                BallNumber = 0,
                Score = 0,
                IsWicketLost = false
            });
            OwnerTotalScoreContainer.UpdateData(0);
            OwnerTotalWicketLostContainer.UpdateData(0);

            HasOtherBattingStarted.UpdateData(false);
            OtherInputScoreContainer.UpdateData(0);
            OtherOverCountContainer.UpdateData(0);
            OtherBallCountContainer.UpdateData(0);
            OtherTurnScoreContainer.UpdateData(0);
            OtherBallDataContainer.UpdateData(new BallData
            {
                OverNumber = 0,
                BallNumber = 0,
                Score = 0,
                IsWicketLost = false
            });
            OtherTotalScoreContainer.UpdateData(0);
            OtherTotalWicketLostContainer.UpdateData(0);
        }

        public void OnBattingStarted()
        {
            if (IsOwnerBatting)
                HasOwnerBattingStarted.UpdateData(true);
            else
                HasOtherBattingStarted.UpdateData(true);
        }

        public bool IsOwnerBatting
        {
            get
            {
                // If this is first half and batsman of first half is owner,
                // OR
                // If this is second half and batsman of fist half was other

                return (CurrentGameStateCategory.Value == GameStateCategory.FirstHalf &&
                    BatsmanOfFirstHalf.Value == PlayerType.OWNER) ||
                (CurrentGameStateCategory.Value == GameStateCategory.SecondHalf &&
                    BatsmanOfFirstHalf.Value == PlayerType.OTHER);
            }
        }
    }
}
