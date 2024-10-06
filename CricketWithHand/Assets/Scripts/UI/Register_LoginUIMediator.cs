using CricketWithHand.Authentication;
using Doozy.Runtime.UIManager.Containers;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Threading;
using System.Threading.Tasks;
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
        private UnityEvent _onLinkAccountSuccess;

        [SerializeField]
        private UnityEvent _onLogOutSuccess;

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
        private int _authenticationTimeOutInSeconds = 10;

        public bool IsLoggedIn => _authServiceFacade.IsLoggedIn;
        public bool IsLinkedInWithGoogle => _authServiceFacade.IsLinkedWithGoogle;

        private PlayFabAuthServiceFacade _authServiceFacade;
        private LoadingUI _loadingUI;
        private CancellationTokenSource _cts;

        private void Start()
        {
            _loadingUI = LoadingUI.instance;
            _authServiceFacade = PlayFabAuthServiceFacade.Instance;

            if (_clearPlayerPrefs)
            {
                _registerUI.Reset();
                _loginUI.Reset();
                _authServiceFacade.ClearCache();
            }

            _loginUI.ToggleRememberMeUI(_authServiceFacade.AuthData.RememberMe);

            // _authServiceFacade.InfoRequestParams = InfoRequestParams;
            // _authServiceFacade.Authenticate();

            TryLoginWithRememberedAccount();
        }

        private void OnDisable()
        {
            CancelAndDisposeCTS();
            LogOut();
        }

        private void TryLoginWithRememberedAccount()
        {
            if (!_authServiceFacade.AuthData.RememberMe)
            {
                return;
            }

            _authServiceFacade.LoginRememberedAccount();
        }

        public void RegisterWithEmailAndPassword(string email, string password, string confirmPassword, bool rememberMe)
        {
            _loadingUI.Show();
            _authServiceFacade.RegisterWithEmailAndPassword(
                email, 
                password, 
                _infoRequestParams,
                rememberMe,
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

        public void LoginWithEmailAndPassword(string email, string password, bool rememberMe)
        {
            _loadingUI.Show();
            _authServiceFacade.AuthenticateEmailPassword(
                email, 
                password, 
                _infoRequestParams,
                rememberMe,

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
            _loadingUI.Show();
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

        public void LoginWithGoogleAccount(bool rememberMe)
        {
            LogUI.instance.AddStatusText("Logging in with google account...");

            _loadingUI.Show();
            StartTimeOutCalculation(_authenticationTimeOutInSeconds);

            _authServiceFacade.AuthenticateWithGoogle(
                _googleWebClientId, 
                _infoRequestParams,
                rememberMe,
                (result) =>
                {
                    LogUI.instance.AddStatusText("PlayFab login with google success!");
                    OnPlayFabLoginSuccess(result);
                    CancelAndDisposeCTS();
                },
                (error) =>
                {
                    OnPlayFabError(error);
                    CancelAndDisposeCTS();
                }
            );
        }

        public void LinkAccountWithGoogle()
        {
            LogUI.instance.AddStatusText("Linking with google ...");

            _loadingUI.Show();
            StartTimeOutCalculation(_authenticationTimeOutInSeconds);

            _authServiceFacade.LinkWithGoogle(
                _googleWebClientId, 
                _infoRequestParams,
                (result) =>
                {
                    LogUI.instance.AddStatusText("PlayFab linked with google success!");
                    OnLinkAccountWithGoogleSuccess();
                    CancelAndDisposeCTS();
                },
                (error) =>
                {
                    OnPlayFabError(error);
                    CancelAndDisposeCTS();
                }
            );
        }

        public void SetDisplayName(string displayName)
        {
            _loadingUI.Show();
            _authServiceFacade.SetDisplayName(
                displayName,
                (userName) => OnUserDisplayNameSet(userName),
                (error) => OnPlayFabError(error)
            );
        }

        public void LogOut()
        {
            if (!_authServiceFacade.IsLoggedIn)
            {
                LogUI.instance.AddStatusText("No user logged in!");
                return;
            }

            if (_authServiceFacade.AuthData.AuthType == Authtypes.Silent)
            {
                ConfirmPopupUI.instance.ShowPopup(
                    new ConfirmPopupUI.Payload()
                    {
                        Title = "WARNING!",
                        Message = "This is a guest account, if you logout from this, \n " +
                        "all datas will be lost, and account can't be recovered. \n" +
                        "Better to link this account before logging out.! \n" +
                        "Do you still want to logout?",
                        LeftButtonLabel = "YES",
                        RightButtonLabel = "NO",
                        OnLeftButtonClickedCallback = () =>
                        {
                            LogOut();
                        }
                    }
                );
            }
            else
            {
                LogOut();
            }


            void LogOut()
            {
                _authServiceFacade.LogOut(
                    (result) =>
                    {
                        LogUI.instance.AddStatusText(result);
                        _onLogOutSuccess?.Invoke();
                    },
                    (error) =>
                    {
                        LogUI.instance.AddStatusText(error);
                    }
                );
            }
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

            _loadingUI.Hide();
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
            PopupUI.instance.ShowPopup($"Error code: {error.Error}", $"Message: {errorReport} \n");

            _loadingUI.Hide();
        }

        private void OnUserDisplayNameSet(string displayName)
        {
            _loadingUI.Hide();
            LogUI.instance.AddStatusText($"Display name set to: . {displayName}");
            _onLoggedInWithDisplayname?.Invoke(displayName);
        }

        private void OnLinkAccountWithGoogleSuccess()
        {
            _loadingUI.Hide();
            PopupUI.instance.ShowPopup("Link with Google Success", "You have successfully linked your account with google!");
            _onLinkAccountSuccess?.Invoke();
        }

        private async void StartTimeOutCalculation(int waitForSeconds)
        {
            // Dispose of the previous CancellationTokenSource if it exists
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            try
            {
                // Wait for the specified time or cancellation
                await Task.Delay(waitForSeconds * 1000, _cts.Token);

                // Check if the token hasn't been canceled before hiding the UI
                if (!_cts.Token.IsCancellationRequested)
                {
                    // Force stop authentication
                    _authServiceFacade.OnAuthenticationTimeOut();
                    _loadingUI.Hide();
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, do nothing as UI is being handled elsewhere
            }
            catch (Exception ex)
            {
                // Log any unexpected errors
                Debug.LogError($"An error occurred: {ex.Message}");
            }
        }

        private void CancelAndDisposeCTS()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
