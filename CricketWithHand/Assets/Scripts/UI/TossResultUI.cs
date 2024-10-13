using CricketWithHand.Gameplay;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class TossResultUI : MonoBehaviour
    {
        [SerializeField]
        private GameConfigSO _gameConfig;

        [SerializeField]
        private TMP_Text _tossResultText;

        [SerializeField]
        private TMP_Text _chooseBatOrBallText;

        public void ShowTossResult(bool ownerOwn)
        {
            if (ownerOwn)
                _tossResultText.text = _gameConfig.OwnerWinTossMessage;
            else
                _tossResultText.text = _gameConfig.OwnerLostTossMessage;
        }

        public void ShowBatOrBallResult(bool willBat)
        {
            if (willBat)
                _chooseBatOrBallText.text = _gameConfig.OwnerWillBatMessage;
            else
                _chooseBatOrBallText.text = _gameConfig.OwnerWillBallMessage;
        }
    }
}
