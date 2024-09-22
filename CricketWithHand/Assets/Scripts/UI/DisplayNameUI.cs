using TMPro;
using UnityEngine;


namespace CricketWithHand.UI
{
    public class DisplayNameUI : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField _displayNameInput;

        [SerializeField]
        Register_LoginUIMediator _registerLoginUIMediator;

        public void SaveDisplayName() =>
            _registerLoginUIMediator.SetDisplayName(_displayNameInput.text);
    }
}
