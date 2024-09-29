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
        private UnityEvent _onLoggedInWithoutDisplayname;

        [SerializeField]
        private UnityEvent _onNoAccountRemembered;

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


        private PlayFabAuthServiceFacade _authServiceFacade = PlayFabAuthServiceFacade.Instance;

        private void Awake()
        {
            if (_clearPlayerPrefs)
            {
                _registerUI.Reset();
                _loginUI.Reset();
                _authServiceFacade.ClearCache();
            }

            _loginUI.SetRememberMeToRememberedState(_authServiceFacade.RememberMe);
        }

        private void Start()
        {
            SubscribeToEvents();

            // _authServiceFacade.InfoRequestParams = InfoRequestParams;
            // _authServiceFacade.Authenticate();

            // TryLoginWithRememberedAccount();
        }

        private void TryLoginWithRememberedAccount()
        {
            if (!_authServiceFacade.RememberMe)
            {
                _onNoAccountRemembered?.Invoke();
                return;
            }

            _authServiceFacade.LoginRememberedAccount();
        }

        private void OnDestroy()
        {
            UnSubscribeFromEvents();
        }

        public void RegisterWithEmailAndPassword(string email, string password, string confirmPassword)
        {
            _loadingView.Show();
            _authServiceFacade.AuthenticateWithEmailAndPassword(email, password, _infoRequestParams, true);
        }

        public void LoginWithEmailAndPassword(string email, string password)
        {
            _loadingView.Show();
            _authServiceFacade.AuthenticateWithEmailAndPassword(email, password, _infoRequestParams, false);
        }

        public void LoginAsAGuest()
        {
            _loadingView.Show();
            _authServiceFacade.AuthenticateAsAGuest(_infoRequestParams);
        }

        public void LoginWithGoogleAccount()
        {
            _loadingView.Show();
            _authServiceFacade.AuthenticateWithGoogle(_googleWebClientId, _infoRequestParams);
        }

        public void LogOut() =>
            _authServiceFacade.LogOut();

        public void ToggleRememberMe(bool toggle) =>
            _authServiceFacade.RememberMe = toggle;

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
                _onLoggedInWithoutDisplayname?.Invoke();
            }

            _loadingView.Hide();
        }

        public void SetDisplayName(string displayName)
        {
            _loadingView.Show();
            _authServiceFacade.SetDisplayName(displayName);
        }

        private void OnPlayFaberror(PlayFabError error)
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
            _onLoggedInWithDisplayname?.Invoke(displayName);
        }

        private void SubscribeToEvents()
        {
            PlayFabAuthServiceFacade.Instance.OnPlayFabLoginSuccess += OnPlayFabLoginSuccess;
            PlayFabAuthServiceFacade.Instance.OnPlayFabError += OnPlayFaberror;
            PlayFabAuthServiceFacade.Instance.OnUserDisplayNameSet += OnUserDisplayNameSet;
        }

        private void UnSubscribeFromEvents()
        {
            PlayFabAuthServiceFacade.Instance.OnPlayFabLoginSuccess -= OnPlayFabLoginSuccess;
            PlayFabAuthServiceFacade.Instance.OnPlayFabError -= OnPlayFaberror;
            PlayFabAuthServiceFacade.Instance.OnUserDisplayNameSet -= OnUserDisplayNameSet;
        }
    }
}
