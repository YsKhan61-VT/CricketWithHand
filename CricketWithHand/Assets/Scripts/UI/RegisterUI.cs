using CricketWithHand.Authentication;
using System.Text;
using TMPro;
using UnityEngine;
using YSK.Utilities;


namespace CricketWithHand.UI
{
    public class RegisterUI : MonoBehaviour
    {
        [SerializeField]
        private Register_LoginUIMediator _registerLoginUIMediator;

        [SerializeField]
        private TMP_InputField _email;

        [SerializeField]
        private TMP_InputField _password;

        [SerializeField]
        private TMP_InputField _confirmPassword;

        [SerializeField]
        private CanvasGroup _otpPanel;

        [SerializeField]
        private TMP_InputField[] _otpInputFields;

        private OTPGenerator _otpGenerator;
        private MailJetServiceFacade _mailServiceFacade;

        private void Start()
        {
            _otpGenerator = new();
            _mailServiceFacade = new();
        }

        public void Reset()
        {
            _email.text = "";
            _password.text = "";
            _confirmPassword.text = "";

            HideOTPPanel();
        }

        public void RegisterUsingEmailAndPassword()
        {
            if (!IsRegistrationCredentialsValid()) return;

            SendOTPForEmailConfirmation();

            ShowOTPPanel();
        }

        public void CheckOTPAndRegister()
        {
            string inputOTP = FetchInputOTPFromOTPInputField();

            if (inputOTP != _otpGenerator.GeneratedOTP)
            {
                LogUI.instance.AddStatusText("OTP not matching!");
                PopupUI.instance.ShowPopup("OTP NOT MATCHING", "Please make sure to type the OTP correctly");
                return;
            }

            HideOTPPanel();

            LogUI.instance.AddStatusText($"Registering ...");
            _registerLoginUIMediator.RegisterWithEmailAndPassword(_email.text, _password.text, _confirmPassword.text);
        }

        private string FetchInputOTPFromOTPInputField()
        {
            StringBuilder otpBuilder = new StringBuilder();

            foreach (TMP_InputField inputField in _otpInputFields)
            {
                otpBuilder.Append(inputField.text);
            }

            return otpBuilder.ToString();
        }

        private void ShowOTPPanel()
        {
            _otpPanel.blocksRaycasts = true;
            _otpPanel.alpha = 1;
        }

        private void HideOTPPanel()
        {
            _otpPanel.blocksRaycasts = false;
            _otpPanel.alpha = 0;
        }

        private void SendOTPForEmailConfirmation()
        {
            _mailServiceFacade.SendEmailAsync(
                new MailJetServiceFacade.ReceiverData()
                {
                    Email = _email.text,
                    Name = null,
                    Subject = "Cricket With Hand Email Verification",
                    Message = $"Your OTP for registering to Cricket With Hand is {_otpGenerator.GenerateOTP()}\n" +
                    $"Please enter this OTP on your app to register successfully!"
                },

                () => { LogUI.instance.AddStatusText("Confirmation Email sent successful!"); },
                (error) => { LogUI.instance.AddStatusText($"Error sending confirmation email: {error}"); }
            );
        }

        private bool IsRegistrationCredentialsValid()
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

            LogUI.instance.AddStatusText($"Credentials valid: {_email.text}, {_password.text}");
            return true;
        }
    }
}
