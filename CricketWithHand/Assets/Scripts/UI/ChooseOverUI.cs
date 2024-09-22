using CricketWithHand.Gameplay;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ChooseOverUI : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        public void SetOneOver() =>
            _turnController.SetTotalOvers(OverCategory.One);

        public void SetTwoOver() =>
            _turnController.SetTotalOvers(OverCategory.Two);

        public void SetFourOvers() =>
            _turnController.SetTotalOvers(OverCategory.Four);

        public void SetSixOvers() =>
            _turnController.SetTotalOvers(OverCategory.Six);

        public void SetToAllOut() =>
            _turnController.SetTotalOvers(OverCategory.AllOut);
    }
}
