using CricketWithHand.PlayFab;
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
        UnityEvent<string> _onLoggedInWithDisplayname;

        [SerializeField]
        UnityEvent _onLoggedInWithoutDisplayname;

        [SerializeField]
        UnityEvent _onNoAccountRemembered;

        #endregion

        [SerializeField]
        RegisterUI _registerUI;

        [SerializeField]
        LoginUI _loginUI;

        [SerializeField]
        bool _clearPlayerPrefs;

        [SerializeField]
        GetPlayerCombinedInfoRequestParams _infoRequestParams;

        [SerializeField]
        string _googleWebClientId;

        PlayFabAuthServiceFacade _authServiceFacade = PlayFabAuthServiceFacade.Instance;

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

        public void RegisterWithEmailAndPassword(string email, string password, string confirmPassword) =>
            _authServiceFacade.AuthenticateWithEmailAndPassword(email, password, _infoRequestParams, true);

        public void LoginWithEmailAndPassword(string email, string password) =>
            _authServiceFacade.AuthenticateWithEmailAndPassword(email, password, _infoRequestParams, false);

        public void LoginAsAGuest() =>
            _authServiceFacade.AuthenticateAsAGuest();

        public void LoginWithGoogleAccount() =>
            _authServiceFacade.AuthenticateWithGoogle(_googleWebClientId);

        public void PlayAsGuest() =>
            _authServiceFacade.AuthenticateAsAGuest();

        public void LogOut() =>
            _authServiceFacade.LogOut();

        public void ToggleRememberMe(bool toggle) =>
            _authServiceFacade.RememberMe = toggle;

        private void OnPlayFabLoginSuccess(LoginResult result)
        {
            LogUI.instance.AddStatusText("Logged In as: {0}" + result.PlayFabId);

            string displayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
            if (displayName != null)
            {
                LogUI.instance.AddStatusText($"Welcome: {displayName}");
                _onLoggedInWithDisplayname?.Invoke(displayName);
            }
            else
            {
                LogUI.instance.AddStatusText("No Display name found!");
                _onLoggedInWithoutDisplayname?.Invoke();
            }
        }

        public void SetDisplayName(string displayName) =>
            _authServiceFacade.SetDisplayName(displayName);

        private void OnPlayFaberror(PlayFabError error)
        {
            //There are more cases which can be caught, below are some
            //of the basic ones.
            switch (error.Error)
            {
                case PlayFabErrorCode.InvalidEmailAddress:
                case PlayFabErrorCode.InvalidPassword:
                case PlayFabErrorCode.InvalidEmailOrPassword:
                case PlayFabErrorCode.InvalidParams:
                case PlayFabErrorCode.AccountNotFound:
                case PlayFabErrorCode.InvalidAccount:
                    LogUI.instance.AddStatusText("Invalid Email or Password");
                    PopupUI.instance.ShowMessage("Authentication Error:", "No account found with the Email. \n Make sure the email address you provided is correct. \n If yes, register with that email address first!");
                    break;
                default:
                    LogUI.instance.AddStatusText(error.GenerateErrorReport());
                    break;
            }
        }

        private void OnPlayFabNoAuthTypeSelected()
        {
            //Here we have choses what to do when AuthType is None.
            /*
             * Optionally we could Not do the above and force login silently
             * 
             * _AuthService.Authenticate(Authtypes.Silent);
             * 
             * This example, would auto log them in by device ID and they would
             * never see any UI for Authentication.
             * 
             */
        }

        private void OnUserDisplayNameSet(string displayName) =>
            _onLoggedInWithDisplayname?.Invoke(displayName);

        private void SubscribeToEvents()
        {
            PlayFabAuthServiceFacade.Instance.OnNoAuthTypeSelected += OnPlayFabNoAuthTypeSelected;
            PlayFabAuthServiceFacade.Instance.OnPlayFabLoginSuccess += OnPlayFabLoginSuccess;
            PlayFabAuthServiceFacade.Instance.OnPlayFabError += OnPlayFaberror;
            PlayFabAuthServiceFacade.Instance.OnUserDisplayNameSet += OnUserDisplayNameSet;
        }

        private void UnSubscribeFromEvents()
        {
            PlayFabAuthServiceFacade.Instance.OnNoAuthTypeSelected -= OnPlayFabNoAuthTypeSelected;
            PlayFabAuthServiceFacade.Instance.OnPlayFabLoginSuccess -= OnPlayFabLoginSuccess;
            PlayFabAuthServiceFacade.Instance.OnPlayFabError -= OnPlayFaberror;
            PlayFabAuthServiceFacade.Instance.OnUserDisplayNameSet -= OnUserDisplayNameSet;
        }
    }
}
