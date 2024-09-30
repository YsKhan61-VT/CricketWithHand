using TMPro;
using UnityEngine;
using YSK.Utilities;


namespace CricketWithHand.UI
{
    public class RegisterUI : MonoBehaviour
    {
        [SerializeField]
        Register_LoginUIMediator _registerLoginUIMediator;

        [SerializeField]
        TMP_InputField _email;

        [SerializeField]
        TMP_InputField _password;

        [SerializeField]
        TMP_InputField _confirmPassword;

        public void Reset()
        {
            _email.text = "";
            _password.text = "";
            _confirmPassword.text = "";
        }

        public void RegisterUsingEmailAndPassword()
        {
            if (!IsRegistrationCredentialsValid()) return;

            LogUI.instance.AddStatusText($"Registering ...");
            _registerLoginUIMediator.RegisterWithEmailAndPassword(_email.text, _password.text, _confirmPassword.text);
        }

        bool IsRegistrationCredentialsValid()
        {
            if (string.IsNullOrEmpty(_email.text))
            {
                LogUI.instance.AddStatusText("Email address can't be empty!");
                PopupUI.instance.ShowMessage("Registration Error", "Email address can't be empty!");
                return false;
            }

            if (string.IsNullOrEmpty(_password.text))
            {
                LogUI.instance.AddStatusText("Password can't be empty!");
                PopupUI.instance.ShowMessage("Registration Error", "Password can't be empty!");
                return false;
            }

            if (string.IsNullOrEmpty(_confirmPassword.text) ||
                _password.text != _confirmPassword.text)
            {
                LogUI.instance.AddStatusText("Password doesn't match.!");
                PopupUI.instance.ShowMessage("Registration Error", "Passwords doesn't match!");
                return false;
            }

            return true;
        }
    }
}
