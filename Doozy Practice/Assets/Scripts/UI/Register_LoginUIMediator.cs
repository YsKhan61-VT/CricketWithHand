using DoozyPractice.PlayFab;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;
using YSK.Utilities;


namespace DoozyPractice.UI
{
    public class Register_LoginUIMediator : MonoBehaviour
    {
        [SerializeField]
        RegisterUI _registerUI;

        [SerializeField]
        LoginUI _loginUI;

        [SerializeField]
        bool _clearPlayerPrefs;

        [SerializeField]
        GetPlayerCombinedInfoRequestParams InfoRequestParams;

        [SerializeField]
        UnityEvent<string> m_OnLoggedInWithDisplayname;

        [SerializeField]
        UnityEvent m_OnLoggedInWithoutDisplayname;

        [SerializeField]
        UnityEvent m_OnNoAccountRemembered;

        PlayFabAuthServiceFacade _authServiceFacade = PlayFabAuthServiceFacade.Instance;

        private void Awake()
        {
            if (_clearPlayerPrefs)
            {
                _registerUI.Reset();
                _loginUI.Reset();
                _authServiceFacade.ClearRememberMe();
                _authServiceFacade.AuthType = Authtypes.None;
            }

            _loginUI.SetRememberMeToRememberedState(_authServiceFacade.RememberMe);
        }

        private void Start()
        {
            PlayFabAuthServiceFacade.OnDisplayAuthentication += OnDisplayAuthentication;
            PlayFabAuthServiceFacade.OnLoginSuccess += OnLoginSuccess;
            PlayFabAuthServiceFacade.OnPlayFabError += OnPlayFaberror;
            PlayFabAuthServiceFacade.OnDisplayNameSet += OnDisplayNameSet;

            _authServiceFacade.InfoRequestParams = InfoRequestParams;
            _authServiceFacade.Authenticate();

            TryLoginWithRememberedAccount();
        }

        void TryLoginWithRememberedAccount()
        {
            if (!_authServiceFacade.RememberMe)
            {
                m_OnNoAccountRemembered?.Invoke();
                return;
            }

            _authServiceFacade.LoginRememberedAccount();
        }

        private void OnDestroy()
        {
            PlayFabAuthServiceFacade.OnDisplayAuthentication -= OnDisplayAuthentication;
            PlayFabAuthServiceFacade.OnLoginSuccess -= OnLoginSuccess;
            PlayFabAuthServiceFacade.OnPlayFabError -= OnPlayFaberror;
            PlayFabAuthServiceFacade.OnDisplayNameSet -= OnDisplayNameSet;
        }

        public void RegisterWithEmailAndPassword(string email, string password)
        {
            _authServiceFacade.Email = email;
            _authServiceFacade.Password = password;
            _authServiceFacade.Authenticate(Authtypes.RegisterPlayFabAccount);
        }

        public void LoginWithEmailAndPassword(string email, string password)
        {
            _authServiceFacade.Email = email;
            _authServiceFacade.Password = password;
            _authServiceFacade.Authenticate(Authtypes.EmailAndPassword);
        }

        public void PlayAsGuest() =>
            _authServiceFacade.Authenticate(Authtypes.Silent);

        public void LogOut() =>
            _authServiceFacade.LogOut();

        public void ToggleRememberMe(bool toggle) =>
            _authServiceFacade.RememberMe = toggle;

        private void OnLoginSuccess(LoginResult result)
        {
            LogUI.Instance.AddStatusText("Logged In as: {0}" + result.PlayFabId);

            PlayFabManager.IsLoggedIn = true;
            string displayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
            if (displayName != null)
            {
                m_OnLoggedInWithDisplayname?.Invoke(displayName);
            }
            else
            {
                m_OnLoggedInWithoutDisplayname?.Invoke();
            }

            // PlayFabManager.LoadUserData();
            // PlayFabManager.LoadAccountData();
            // PlayFabManager.LoadTitleData();
        }

        public void SetDisplayName(string displayName) =>
            _authServiceFacade.SetDisplayName(displayName);

        void OnPlayFaberror(PlayFabError error)
        {
            //There are more cases which can be caught, below are some
            //of the basic ones.
            switch (error.Error)
            {
                case PlayFabErrorCode.InvalidEmailAddress:
                case PlayFabErrorCode.InvalidPassword:
                case PlayFabErrorCode.InvalidEmailOrPassword:
                    LogUI.Instance.AddStatusText("Invalid Email or Password");
                    break;

                case PlayFabErrorCode.AccountNotFound:
                    return;
                default:
                    LogUI.Instance.AddStatusText(error.GenerateErrorReport());
                    break;
            }
        }

        void OnDisplayAuthentication()
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

        void OnDisplayNameSet(string displayName)
        {
            m_OnLoggedInWithDisplayname?.Invoke(displayName);
        }
    }
}
