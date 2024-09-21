using DoozyPractice.Gameplay;
using UnityEngine;


namespace DoozyPractice.UI
{
    public class ChooseOverUI : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        public void SetTotalOvers(int totalOvers) => 
            _turnController.SetTotalOvers(totalOvers);
    }
}
