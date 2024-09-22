using CricketWithHand.Gameplay;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ChooseWicketUI : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        public void SetWicket(int count) =>
            _turnController.SetTotalWickets(count);
    }
}
