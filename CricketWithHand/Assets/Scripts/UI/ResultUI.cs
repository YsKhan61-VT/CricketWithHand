using TMPro;
using UnityEngine;


namespace DoozyPractice.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text _resultMessageText;

        [SerializeField]
        string _winMessage;

        [SerializeField]
        string _lossMessage;

        [SerializeField]
        string _drawMessage;

        public void ShowWinText() =>
            _resultMessageText.text = _winMessage;

        public void ShowLossText() =>
            _resultMessageText.text = _lossMessage;

        public void ShowDrawText() =>
            _resultMessageText.text = _drawMessage;
    }
}
