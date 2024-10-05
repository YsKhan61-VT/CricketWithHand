using CricketWithHand.Authentication;
using Doozy.Runtime.UIManager.Components;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private LoadingUI _loadingUI;

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

        [SerializeField]
        private UIButton _resendOTPButton;

        [SerializeField]
        private TMP_Text _resendOTPText;

        private OTPGenerator _otpGenerator;
        private PlayFabAuthServiceFacade _playFabAuthServiceFacade;
        private MailJetServiceFacade _mailServiceFacade;
        private CancellationTokenSource _cts;

        private void Start()
        {
            _otpGenerator = new();
            _mailServiceFacade = new();
            _playFabAuthServiceFacade = PlayFabAuthServiceFacade.Instance;

            HideOTPPanel();

            _resendOTPText.text = "";
            DisableResendOTPButton();
        }

        public void Reset()
        {
            _email.text = "";
            _password.text = "";
            _confirmPassword.text = "";
        }

        public void RegisterUsingEmailAndPassword()
        {
            if (!IsRegistrationCredentialsValid()) return;

            _loadingUI.Show();

            _mailServiceFacade.SendEmailAsync(
                new MailJetServiceFacade.ReceiverData()
                {
                    Email = _email.text,
                    Name = null,
                    Subject = "Cricket With Hand Email Verification",
                    Message = $"Your OTP for registering to Cricket With Hand is {_otpGenerator.GenerateOTP()}\n" +
                    $"Please enter this OTP on your app to register successfully!"
                },

                () =>
                {
                    LogUI.instance.AddStatusText("Confirmation Email sent successful!");
                    _loadingUI.Hide();
                    ShowOTPPanel();

                    UpdateResendOTPTextUI();
                },

                (error) =>
                {
                    LogUI.instance.AddStatusText($"Error verifying Email address!: {error}");
                    PopupUI.instance.ShowPopup("Error verifying Email address!", "Please try again!");
                }
            );
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

        public void HideOTPPanel()
        {
            _otpPanel.blocksRaycasts = false;
            _otpPanel.alpha = 0;
        }

        public void DeactivateResendOTPTextUI()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async void UpdateResendOTPTextUI()
        {
            // Cancel any previous countdown task
            _cts?.Cancel();

            // Check if the user can regenerate the OTP
            if (_otpGenerator.CanRegenerateOTP())
            {
                _resendOTPText.text = "";
                EnableResendOTPButton();
                return;
            }

            // Create a new cancellation token source for the countdown
            _cts = new CancellationTokenSource();

            try
            {
                // Start a loop that updates the UI until the user can regenerate OTP
                while (!_otpGenerator.CanRegenerateOTP())
                {
                    TimeSpan remainingTime = _otpGenerator.RemainingSecondsToRegenerateOTP();

                    // Display the countdown as MM:SS
                    _resendOTPText.text = $"Resend after {remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";

                    // Wait for 1 second asynchronously
                    await Task.Delay(1000, _cts.Token);
                }

                // Once the countdown is complete, update the text to allow resend
                _resendOTPText.text = "";
                EnableResendOTPButton();
            }
            catch (TaskCanceledException e)
            {
                LogUI.instance.AddStatusText(e.ToString());
            }
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

        private void EnableResendOTPButton() =>
            _resendOTPButton.enabled = true;

        private void DisableResendOTPButton() =>
            _resendOTPButton.enabled = false;

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
                PopupUI.instance.ShowPopup("Invalid Password", "Password can't be empty!");
                return false;
            }

            if (!_playFabAuthServiceFacade.IsPasswordValid(_password.text))
            {
                LogUI.instance.AddStatusText("Password should contain in between 6 to 15 characters!");
                PopupUI.instance.ShowPopup("Invalid Password", "Password should contain in between 6 to 15 characters!");
                return false;
            }

            if (string.IsNullOrEmpty(_confirmPassword.text) ||
                _password.text != _confirmPassword.text)
            {
                LogUI.instance.AddStatusText("Password doesn't match.!");
                PopupUI.instance.ShowPopup("Invalid Password", "Passwords doesn't match!");
                return false;
            }

            LogUI.instance.AddStatusText($"Credentials valid: {_email.text}, {_password.text}");
            return true;
        }
    }
}
