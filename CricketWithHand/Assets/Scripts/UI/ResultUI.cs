using CricketWithHand.Utility;
using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField]
        private GameConfigSO _gameConfig;

        [SerializeField]
        TMP_Text _resultMessageText;

        public void ShowWinText() =>
            _resultMessageText.text = _gameConfig.WinMessage;

        public void ShowLossText() =>
            _resultMessageText.text = _gameConfig.LossMessage;

        public void ShowDrawText() =>
            _resultMessageText.text = _gameConfig.DrawMessage;
    }
}
