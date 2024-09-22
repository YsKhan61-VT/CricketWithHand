using DoozyPractice.Gameplay;
using UnityEngine;


namespace DoozyPractice.UI
{
    public class OwnerChooseBatOrBallInputUI : MonoBehaviour
    {
        [SerializeField]
        TossController _tossController;

        public void ChooseToBat() =>
            _tossController.ChooseBatOrBallFirst(true, true);

        public void ChooseToBall() =>
            _tossController.ChooseBatOrBallFirst(true, false);
    }
}
