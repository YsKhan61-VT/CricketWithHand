using DoozyPractice.UI;
using UnityEngine;
using UnityEngine.Events;


namespace DoozyPractice.Gameplay
{
    public class TossController : MonoBehaviour
    {
        [SerializeField]
        UnityEvent OnOwnerOwnToss;

        [SerializeField]
        UnityEvent OnOwnerLostToss;

        [SerializeField]
        UnityEvent OnPublishTossResult;

        [SerializeField]
        TurnController _turnController;

        [SerializeField]
        TossResultUI _tossResultUI;

        [SerializeField, Range(0f, 1f)]
        float _ownerWinningChance;

        private bool _ownerWonToss;

        public void Toss()
        {
            _ownerWonToss = Random.Range(0f, 1f) <= _ownerWinningChance;
            if (_ownerWonToss)
            {
                OnOwnerOwnToss?.Invoke();
            }
            else
            {
                OnOwnerLostToss?.Invoke();
            }

            _tossResultUI.ShowTossResult(_ownerWonToss);
        }

        public void ChooseBatOrBallFirst(bool isOwner, bool willBat)
        {
            if ((isOwner && willBat) ||
                (!isOwner && !willBat))
            {
                // either owner choose to bat, or non-owner choose to ball,
                // in both cases, owner have to bat.
                _turnController.ChangeBatsman(true);
                _tossResultUI.ShowBatOrBallResult(true);
            }
            else
            {
                // otherwise, owner have to ball.
                _turnController.ChangeBatsman(false);
                _tossResultUI.ShowBatOrBallResult(false);
            }

            OnPublishTossResult?.Invoke();
        }
    }
}
