using CricketWithHand.Authentication;
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
        MailJetServiceFacade _mailJetServiceFacade;

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
            // if (!IsRegistrationCredentialsValid()) return;

            // LogUI.instance.AddStatusText($"Registering ...");
            // _registerLoginUIMediator.RegisterWithEmailAndPassword(_email.text, _password.text, _confirmPassword.text);

            SendTestMail();
        }

        void SendTestMail()
        {
            _mailJetServiceFacade.SendEmailAsync(
                new MailJetServiceFacade.ReceiverData()
                {
                    Email = _email.text,
                    Name = null,
                    Subject = "Test",
                    Message = "This is a test API to check if the MailJet API is working"
                },

                () => { LogUI.instance.AddStatusText("Confirmation Email sent successful!"); },
                (error) => { LogUI.instance.AddStatusText($"Error sending confirmation email: {error}"); }
            );
        }

        bool IsRegistrationCredentialsValid()
        {
            if (string.IsNullOrEmpty(_email.text))
            {
                LogUI.instance.AddStatusText("Email address can't be empty!");
                PopupUI.instance.ShowPopup("Registration Error", "Email address can't be empty!");
                return false;
            }

            if (string.IsNullOrEmpty(_password.text))
            {
                LogUI.instance.AddStatusText("Password can't be empty!");
                PopupUI.instance.ShowPopup("Registration Error", "Password can't be empty!");
                return false;
            }

            if (string.IsNullOrEmpty(_confirmPassword.text) ||
                _password.text != _confirmPassword.text)
            {
                LogUI.instance.AddStatusText("Password doesn't match.!");
                PopupUI.instance.ShowPopup("Registration Error", "Passwords doesn't match!");
                return false;
            }

            return true;
        }
    }
}
