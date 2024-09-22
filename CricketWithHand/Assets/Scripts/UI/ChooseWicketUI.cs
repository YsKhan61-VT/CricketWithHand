using DoozyPractice.Gameplay;
using UnityEngine;


namespace DoozyPractice.UI
{
    public class ChooseWicketUI : MonoBehaviour
    {
        [SerializeField]
        TurnController _turnController;

        public void SetWicket(int count) =>
            _turnController.SetTotalWickets(count);
    }
}
