using CricketWithHand.Utility;
using UnityEngine;


namespace CricketWithHand.Gameplay
{
    public enum GameResultEnum
    {
        WON,
        LOST,
        DRAW
    }

    [CreateAssetMenu(fileName = "GameResultDataContainer", menuName = "ScriptableObjects/DataContainers/GameResultDataContainerSO")]
    public class GameResultDataContainerSO : GenericDataContainerSO<GameResultEnum> { }
}
