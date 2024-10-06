using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;
using YSK.Utilities;
using static Fusion.Editor.FusionHubWindow;


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
            _registerLoginUIMediator.LoginWithEmailAndPassword(
                _emailInputField.text, 
                _passwordInputField.text,
                _rememberMeToggle.isOn);
        }

        public void LoginAsGuest()
        {
            LogUI.instance.AddStatusText("$Loging in as a guest user!...");
            _registerLoginUIMediator.LoginAsAGuest();
        }

        public void LoginWithGoogle()
        {
            _registerLoginUIMediator.LoginWithGoogleAccount(_rememberMeToggle.isOn);
        }

        public void LoginWithFacebook()
        {

        }

        public void ToggleRememberMeUI(bool value) =>
            _rememberMeToggle.isOn = value;

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
                PopupUI.instance.ShowPopup("Login Error", "Email address can't be empty!");
                return false;
            }

            if (string.IsNullOrEmpty(_passwordInputField.text))
            {
                LogUI.instance.AddStatusText("Password can't be empty!");
                PopupUI.instance.ShowPopup("Login Error", "Password can't be empty!");
                return false;
            }

            return true;
        }
    }
}
