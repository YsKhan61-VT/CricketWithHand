using CricketWithHand.PlayFab;
using Doozy.Runtime.UIManager.Containers;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;
using YSK.Utilities;


namespace CricketWithHand.UI
{
    public class Register_LoginUIMediator : MonoBehaviour
    {
        #region Events

        [SerializeField]
        private UnityEvent<string> _onLoggedInWithDisplayname;

        [SerializeField]
        private UnityEvent _onLoggedInWithoutDisplayName;

        [SerializeField]
        private UnityEvent _onNoAccountRemembered;

        [SerializeField]
        private UnityEvent _onLinkAccountSuccess;

        #endregion


        [SerializeField]
        private RegisterUI _registerUI;

        [SerializeField]
        private LoginUI _loginUI;

        [SerializeField]
        private bool _clearPlayerPrefs;

        [SerializeField]
        private GetPlayerCombinedInfoRequestParams _infoRequestParams;

        [SerializeField]
        private string _googleWebClientId;

        [SerializeField]
        private UIView _loadingView;

        public bool IsLoggedIn => _authServiceFacade.AuthData.IsLoggedIn;
        public bool IsLinkedInWithGoogle => _authServiceFacade.IsLinkedWithGoogle;

        private PlayFabAuthServiceFacade _authServiceFacade = PlayFabAuthServiceFacade.Instance;

        private void Awake()
        {
            if (_clearPlayerPrefs)
            {
                _registerUI.Reset();
                _loginUI.Reset();
                _authServiceFacade.ClearCache();
            }

            _loginUI.SetRememberMeToRememberedState(_authServiceFacade.AuthData.RememberMe);
        }

        private void Start()
        {
            // _authServiceFacade.InfoRequestParams = InfoRequestParams;
            // _authServiceFacade.Authenticate();

            // TryLoginWithRememberedAccount();
        }

        private void TryLoginWithRememberedAccount()
        {
            if (!_authServiceFacade.AuthData.RememberMe)
            {
                _onNoAccountRemembered?.Invoke();
                return;
            }

            _authServiceFacade.LoginRememberedAccount();
        }

        public void RegisterWithEmailAndPassword(string email, string password, string confirmPassword)
        {
            _loadingView.Show();
            _authServiceFacade.RegisterWithEmailAndPassword(
                email, 
                password, 
                _infoRequestParams,
                (result) =>
                {
                    OnPlayFabLoginSuccess(result);
                },
                (error) =>
                {
                    OnPlayFabError(error);
                }
            );
        }

        public void LoginWithEmailAndPassword(string email, string password)
        {
            _loadingView.Show();
            _authServiceFacade.AuthenticateEmailPassword(
                email, 
                password, 
                _infoRequestParams,
                (result) =>
                {
                    OnPlayFabLoginSuccess(result);
                },
                (error) =>
                {
                    OnPlayFabError(error);
                }
            );
        }

        public void LoginAsAGuest()
        {
            _loadingView.Show();
            _authServiceFacade.SilentlyAuthenticate(
                _infoRequestParams,
                (result) =>
                {
                    OnPlayFabLoginSuccess(result);
                },
                (error) =>
                {
                    OnPlayFabError(error);
                } 
            );
        }

        public void LoginWithGoogleAccount()
        {
            LogUI.instance.AddStatusText("Logging in with google account...");

            _loadingView.Show();
            _authServiceFacade.AuthenticateWithGoogle(
                _googleWebClientId, 
                _infoRequestParams,
                (result) =>
                {
                    LogUI.instance.AddStatusText("PlayFab login with google success!");
                    OnPlayFabLoginSuccess(result);
                },
                (error) =>
                {
                    OnPlayFabError(error);
                }
            );
        }

        public void LinkAccountWithGoogle()
        {
            LogUI.instance.AddStatusText("Linking with google ...");

            _loadingView.Show();
            _authServiceFacade.LinkWithGoogle(
                _googleWebClientId, 
                _infoRequestParams,
                (result) =>
                {
                    LogUI.instance.AddStatusText("PlayFab linked with google success!");
                    OnLinkAccountWithGoogleSuccess();
                },
                (error) =>
                {
                    OnPlayFabError(error);
                }
            );
        }

        public void SetDisplayName(string displayName)
        {
            _loadingView.Show();
            _authServiceFacade.SetDisplayName(
                displayName,
                (userName) => OnUserDisplayNameSet(userName),
                (error) => OnPlayFabError(error)
            );
        }

        /// <summary>
        /// Rather than settting like this, directly set this bool value 
        /// while calling actual authentication methods
        /// </summary>
        /// <param name="result"></param>
        /*public void ToggleRememberMe(bool toggle) =>
            _authServiceFacade.AuthData.RememberMe = toggle;*/

        public void LogOut()
        {
            _authServiceFacade.LogOut(
                (result) => 
                {
                    LogUI.instance.AddStatusText(result);
                },
                (error) =>
                {
                    LogUI.instance.AddStatusText(error);
                }
            );
        }

        private void OnPlayFabLoginSuccess(LoginResult result)
        {
            LogUI.instance.AddStatusText($"Logged In as: {result.PlayFabId}");

            string displayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
            if (displayName != null)
            {
                LogUI.instance.AddStatusText($"Welcome: {displayName}");
                _onLoggedInWithDisplayname?.Invoke(displayName);
            }
            else
            {
                LogUI.instance.AddStatusText("User display name needs to be set!");
                _onLoggedInWithoutDisplayName?.Invoke();
            }

            _loadingView.Hide();
        }

        private void OnPlayFabError(PlayFabError error)
        {
            /*//There are more cases which can be caught, below are some
            //of the basic ones.
            switch (error.Error)
            {
                case PlayFabErrorCode.InvalidEmailAddress:
                    LogUI.instance.AddStatusText($"Error Code: {error.Error} Invalid Email");
                    PopupUI.instance.ShowMessage("Authentication Error:", "No account found with the Email. \n Make sure the email address you provided is correct. \n If yes, register with that email address first!");
                    break;
                case PlayFabErrorCode.InvalidPassword:
                    LogUI.instance.AddStatusText("Invalid password");
                    PopupUI.instance.ShowMessage("Authentication Error:", "No account found with the Email. \n Make sure the email address you provided is correct. \n If yes, register with that email address first!");
                case PlayFabErrorCode.InvalidEmailOrPassword:
                case PlayFabErrorCode.InvalidParams:
                case PlayFabErrorCode.AccountNotFound:
                case PlayFabErrorCode.InvalidAccount:
                    LogUI.instance.AddStatusText("Invalid Email or Password");
                    PopupUI.instance.ShowMessage("Authentication Error:", "No account found with the Email. \n Make sure the email address you provided is correct. \n If yes, register with that email address first!");
                    break;
                default:
                    LogUI.instance.AddStatusText($"{error.Error}, {error.ErrorMessage}");
                    break;
            }*/

            string errorReport = error.GenerateErrorReport();
            LogUI.instance.AddStatusText($"Error code: {error.Error} \n Message: {errorReport} \n");
            PopupUI.instance.ShowMessage($"Error code: {error.Error}", $"Message: {errorReport} \n");

            _loadingView.Hide();
        }

        private void OnUserDisplayNameSet(string displayName)
        {
            _loadingView.Hide();
            LogUI.instance.AddStatusText($"Display name set to: . {displayName}");
            _onLoggedInWithDisplayname?.Invoke(displayName);
        }

        private void OnLinkAccountWithGoogleSuccess()
        {
            _loadingView.Hide();
            PopupUI.instance.ShowMessage("Link with Google Success", "You have successfully linked your account with google!");
            _onLinkAccountSuccess?.Invoke();
        }
    }
}
