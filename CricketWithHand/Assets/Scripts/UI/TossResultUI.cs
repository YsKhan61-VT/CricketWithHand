using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class TossResultUI : MonoBehaviour
    {
        [SerializeField]
        string _ownerWinMessage = "You won the toss!";

        [SerializeField]
        string _ownerLostMessage = "You lost the toss!";

        [SerializeField]
        string _ownerWillBatMessage = "You will be batting.";

        [SerializeField]
        string _ownerWillBallMessage = "You will be balling.";

        [SerializeField]
        TMP_Text _tossResultText;

        [SerializeField]
        TMP_Text _chooseBatOrBallText;

        public void ShowTossResult(bool ownerOwn)
        {
            if (ownerOwn)
                _tossResultText.text = _ownerWinMessage;
            else
                _tossResultText.text = _ownerLostMessage;
        }

        public void ShowBatOrBallResult(bool willBat)
        {
            if (willBat)
                _chooseBatOrBallText.text = _ownerWillBatMessage;
            else
                _chooseBatOrBallText.text = _ownerWillBallMessage;
        }
    }
}
