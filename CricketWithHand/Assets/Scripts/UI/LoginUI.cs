using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;
using YSK.Utilities;


namespace CricketWithHand.UI
{
    public class LoginUI : MonoBehaviour
    {
        [SerializeField]
        Register_LoginUIMediator _registerLoginUIMediator;

        [SerializeField]
        TMP_InputField _emailInputField;

        [SerializeField]
        TMP_InputField _passwordInputField;

        [SerializeField]
        UIToggle _rememberMeToggle;

        public void LoginWithEmailAndPassword()
        {
            if (!IsLoginCredentialsValid()) return;

            LogUI.instance.AddStatusText($"Loging in ...");
            _registerLoginUIMediator.LoginWithEmailAndPassword(_emailInputField.text, _passwordInputField.text);
        }

        public void LoginAsGuest()
        {
            LogUI.instance.AddStatusText("$Loging in as a guest user!...");
            _registerLoginUIMediator.LoginAsAGuest();
        }

        public void LoginWithGoogle()
        {
            _registerLoginUIMediator.LoginWithGoogleAccount();
        }

        public void LoginWithFacebook()
        {

        }

        public void SetRememberMeToRememberedState(bool state) =>
            _rememberMeToggle.isOn = state;

        public void ToggleRememberMe(bool toggle) { }
            // _registerLoginUIMediator.ToggleRememberMe(toggle);

        public void Reset()
        {
            _emailInputField.text = "";
            _passwordInputField.text = "";
        }

        bool IsLoginCredentialsValid()
        {
            if (string.IsNullOrEmpty(_emailInputField.text))
            {
                LogUI.instance.AddStatusText("Email address can't be empty!");
                return false;
            }

            if (string.IsNullOrEmpty(_passwordInputField.text))
            {
                LogUI.instance.AddStatusText("Password can't be empty!");
                return false;
            }

            return true;
        }
    }
}
