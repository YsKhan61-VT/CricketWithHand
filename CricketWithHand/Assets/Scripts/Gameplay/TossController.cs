using CricketWithHand.UI;
using CricketWithHand.Utility;
using UnityEngine;
using UnityEngine.Events;


namespace CricketWithHand.Gameplay
{
    public class TossController : MonoBehaviour
    {
        [SerializeField]
        private GameConfigSO _gameConfig;

        [SerializeField]
        private BatsmanDataContainerSO _batsmanOfFirstHalf;

        [SerializeField]
        private UnityEvent OnOwnerOwnToss;

        [SerializeField]
        private UnityEvent OnOwnerLostToss;

        [SerializeField]
        private UnityEvent OnPublishTossResult;

        [SerializeField]
        private TossResultUI _tossResultUI;

        

        private bool _ownerWonToss;

        public void Toss()
        {
            _ownerWonToss = Random.Range(0f, 1f) <= _gameConfig.OwnerWinningChance;
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

                _batsmanOfFirstHalf.UpdateData(PlayerType.OWNER);
                _tossResultUI.ShowBatOrBallResult(true);
            }
            else
            {
                // otherwise, owner have to ball.

                _batsmanOfFirstHalf.UpdateData(PlayerType.OTHER);
                _tossResultUI.ShowBatOrBallResult(false);
            }

            OnPublishTossResult?.Invoke();
        }
    }
}
