using CricketWithHand.Utility;
using UnityEngine;


namespace CricketWithHand.Gameplay
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/DataContainers/GameDataSO")]
    public class GameDataSO : ScriptableObject
    {
        public GameStateCategoryDataContainerSO CurrentGameStateCategory;

        public PlayerTypeDataContainerSO BatsmanOfFirstHalf;

        public IntDataContainerSO TotalOversCountDataContainer;

        public IntDataContainerSO TotalWicketsCountDataContainer;

        public FloatDataContainerSO TurnSliderValue;

        public PlayerTypeDataContainerSO Winner;

        public void ResetData()
        {
            TurnSliderValue.UpdateData(1);
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
