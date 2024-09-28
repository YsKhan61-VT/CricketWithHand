//--------------------------------------------------------------------------------------
// PlayFabAuthServiceFacade.cs
//
// A Facade to simplify and abstract PlayFab API for authentication services
//--------------------------------------------------------------------------------------

using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using LoginResult = PlayFab.ClientModels.LoginResult;
using System;
using YSK.Utilities;
using CricketWithHand.PlayFab.Google;
using Google;



namespace CricketWithHand.PlayFab
{
    /// <summary>
    /// Supported Authentication types
    /// See - https://api.playfab.com/documentation/client#Authentication
    /// </summary>
    public enum Authtypes
    {
        None,
        Silent,
        UsernameAndPassword,
        EmailAndPassword,
        GooglePlay
    }

    public class PlayFabAuthServiceFacade
    {
        #region Events

        /// <summary>
        /// Invoked when user can successfully login with Playfab 
        /// (using either 3rd party authentication or silently).
        /// This will be invoked either with or without the UserDisplayName.
        /// LoginResult stores all the login related informations.
        /// </summary>
        public event Action<LoginResult> OnPlayFabLoginSuccess;

        /// <summary>
        /// Invoked when there is some error in the process of authentications.
        /// PlayFabError contains the error data.
        /// </summary>
        public event Action<PlayFabError> OnPlayFabError;

        /// <summary>
        /// Invoked when the user display name is set to playfab.
        /// string contains the User Display Name
        /// </summary>
        public event Action<string> OnUserDisplayNameSet;

        #endregion


        #region Constants

        private const string _LoginRememberKey = "PlayFabLoginRemember";
        private const string _PlayFabRememberMeIdKey = "PlayFabIdPassGuid";
        private const string _PlayFabAuthTypeKey = "PlayFabAuthType";

        #endregion

        // These are fields that we set when we are using the service.
        public string Email { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string AuthTicket { get; private set; }

        public GetPlayerCombinedInfoRequestParams InfoRequestParams { get; private set; }

        // This is a force link flag for custom ids for demoing
        public bool ForceLink { get; private set; } = false;

        // Accessbility for PlayFab ID & Session Tickets
        public bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();
        public string PlayFabId { get; private set; }
        public string SessionTicket { get; private set; }
        public string UserDisplayName { get; private set; }


        private GoogleAuthentication _googleAuth;

        public static PlayFabAuthServiceFacade Instance
        {
            get
            {
                _instance ??= new PlayFabAuthServiceFacade();
                return _instance;
            }
        }

        private static PlayFabAuthServiceFacade _instance;

        public PlayFabAuthServiceFacade()
        {
            _instance = this;
        }

        /// <summary>
        /// Remember the user next time they log in
        /// This is used for Auto-Login purpose.
        /// </summary>
        public bool RememberMe
        {
            get
            {
                return PlayerPrefs.GetInt(_LoginRememberKey, 0) != 0;
            }
            set
            {
                PlayerPrefs.SetInt(_LoginRememberKey, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Remember the type of authenticate for the user
        /// </summary>
        public Authtypes AuthType
        {
            get
            {
                return (Authtypes)PlayerPrefs.GetInt(_PlayFabAuthTypeKey, 0);
            }
            private set
            {
                PlayerPrefs.SetInt(_PlayFabAuthTypeKey, (int)value);
            }
        }

        /// <summary>
        /// Generated Remember Me ID
        /// Pass Null for a value to have one auto-generated.
        /// </summary>
        private string RememberMeId
        {
            get
            {
                return PlayerPrefs.GetString(_PlayFabRememberMeIdKey, "");
            }
            set
            {
                var guid = value ?? Guid.NewGuid().ToString();
                PlayerPrefs.SetString(_PlayFabRememberMeIdKey, guid);
            }
        }

        public void ClearCache()
        {
            AuthType = Authtypes.None;

            PlayerPrefs.DeleteKey(_LoginRememberKey);
            PlayerPrefs.DeleteKey(_PlayFabRememberMeIdKey);
            PlayerPrefs.DeleteKey(_PlayFabAuthTypeKey);
        }

        public void AuthenticateWithEmailAndPassword(
            string email, 
            string password, 
            GetPlayerCombinedInfoRequestParams infoRequestParams,
            bool createAccount)
        {
            AuthType = Authtypes.EmailAndPassword;
            Email = email;
            Password = password;
            InfoRequestParams = infoRequestParams;

            if (createAccount)
                RegisterAccountWithEmailAndPassword();
            else
                AuthenticateEmailPassword();
        }

        public void AuthenticateAsAGuest(
            GetPlayerCombinedInfoRequestParams infoRequestParams)
        {
            InfoRequestParams = infoRequestParams;
            AuthType = Authtypes.Silent;
            SilentlyAuthenticate();
        }

        public void AuthenticateWithGoogle(
            string webClientId,
            GetPlayerCombinedInfoRequestParams infoRequestParams)
        {
            InfoRequestParams = infoRequestParams;
            AuthType = Authtypes.GooglePlay;
            _googleAuth ??= new();
            _googleAuth.OnSignInSuccess += OnSignInSuccessWithGoogle;
            _googleAuth.OnSignInFailure += OnSignInFailureWithGoogle;
            _googleAuth.SignInWithGoogle(webClientId);
        }

        public void SetDisplayName(string displayName)
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(
                new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = displayName
                },

                (UpdateUserTitleDisplayNameResult result) =>
                {
                    UserDisplayName = result.DisplayName;
                    LogUI.instance.AddStatusText($"Display name set to: . {UserDisplayName}");
                    OnUserDisplayNameSet?.Invoke(UserDisplayName);
                },

                (PlayFabError error) =>
                {
                    LogUI.instance.AddStatusText(error.GenerateErrorReport());
                    OnPlayFabError?.Invoke(error);
                });
        }

        public void LoginRememberedAccount()
        {
            //Check if the users has opted to be remembered.
            if (RememberMe && !string.IsNullOrEmpty(RememberMeId))
            {
                // If the user is being remembered, then log them in with a customid that was 
                // generated by the RememberMeId property
                PlayFabClientAPI.LoginWithCustomID(
                    new LoginWithCustomIDRequest()
                    {
                        TitleId = PlayFabSettings.TitleId,
                        CustomId = RememberMeId,
                        CreateAccount = true,
                        InfoRequestParameters = InfoRequestParams
                    },

                    // Success
                    (LoginResult result) =>
                    {
                        //Store identity and session
                        PlayFabId = result.PlayFabId;
                        SessionTicket = result.SessionTicket;

                        OnPlayFabLoginSuccess?.Invoke(result);
                    },

                    // Failure
                    (PlayFabError error) =>
                    {
                        OnPlayFabError?.Invoke(error);
                    }
                );
            }
        }

        public void LogOut()
        {
            if (!IsLoggedIn)
            {
                LogUI.instance.AddStatusText("No user is logged in!");
                return;
            }

            switch (AuthType)
            {
                case Authtypes.GooglePlay:
                    if (_googleAuth == null || !_googleAuth.IsLoggedIn)
                        return;
                    _googleAuth.SignOut();
                    break;

                case Authtypes.Silent:
                    LogUI.instance.AddStatusText($"Unlinking device ID...!");
                    UnlinkSilentAuth();
                    break;
                case Authtypes.None:
                    LogUI.instance.AddStatusText("THIS SHOULD NOT HAPPEN!");
                    break;
            }

            PlayFabClientAPI.ForgetAllCredentials();
            LogUI.instance.AddStatusText("Logged out of PlayFab!");
        }

        /// <summary>
        /// Authenticate a user in PlayFab using an Email & Password combo
        /// </summary>
        private void AuthenticateEmailPassword()
        {
            // We have not opted for remember me in a previous session, so now we have to login the user with email & password.
            PlayFabClientAPI.LoginWithEmailAddress(
                new LoginWithEmailAddressRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    Email = Email,
                    Password = Password,
                    InfoRequestParameters = InfoRequestParams
                },

                // Success
                (LoginResult result) =>
                {
                    PlayFabId = result.PlayFabId;
                    SessionTicket = result.SessionTicket;

                    // Note: At this point, they already have an account with PlayFab using a Username (email) & Password
                    // If RememberMe is checked, then generate a new Guid for Login with CustomId.
                    if (RememberMe)
                    {
                        RememberMeId = Guid.NewGuid().ToString();
                        AuthType = Authtypes.EmailAndPassword;

                        // Fire and forget, but link a custom ID to this PlayFab Account.
                        PlayFabClientAPI.LinkCustomID(
                            new LinkCustomIDRequest
                            {
                                CustomId = RememberMeId,
                                ForceLink = ForceLink
                            },
                            null,   // Success callback
                            null    // Failure callback
                            );
                    }

                    OnPlayFabLoginSuccess?.Invoke(result);
                },

                // Failure
                (PlayFabError error) =>
                {
                    OnPlayFabError?.Invoke(error);
                });
        }

        /// <summary>
        /// Register a user with an Email & Password
        /// Note: We are not using the RegisterPlayFab API
        /// </summary>
        private void RegisterAccountWithEmailAndPassword()
        {
            // Any time we attempt to register a player, first silently authenticate the player.
            // This will retain the players True Origination (Android, iOS, Desktop)
            SilentlyAuthenticate(
                (LoginResult result) =>
                {
                    if (result == null)
                    {
                        //something went wrong with Silent Authentication, Check the debug console.
                        OnPlayFabError?.Invoke(new PlayFabError()
                        {
                            Error = PlayFabErrorCode.UnknownError,
                            ErrorMessage = "Silent Authentication by Device failed"
                        });
                    }

                    // Note: If silent auth is success, which is should always be and the following 
                    // below code fails because of some error returned by the server ( like invalid email or bad password )
                    // this is okay, because the next attempt will still use the same silent account that was already created.

                    // Now add our username & password.
                    PlayFabClientAPI.AddUsernamePassword(
                        new AddUsernamePasswordRequest()
                        {
                            Username = Username ?? result.PlayFabId, // Because it is required & Unique and not supplied by User.
                            Email = Email,
                            Password = Password,
                        },

                        // Success
                        (AddUsernamePasswordResult addResult) =>
                        {
                            // Store identity and session
                            PlayFabId = result.PlayFabId;
                            SessionTicket = result.SessionTicket;

                            // If they opted to be remembered on next login.
                            if (RememberMe)
                            {
                                // Generate a new Guid 
                                RememberMeId = Guid.NewGuid().ToString();

                                // Fire and forget, but link the custom ID to this PlayFab Account.
                                PlayFabClientAPI.LinkCustomID(
                                    new LinkCustomIDRequest()
                                    {
                                        CustomId = RememberMeId,
                                        ForceLink = ForceLink
                                    },
                                    // success
                                    null,
                                    // error
                                    null
                                    );
                            }

                            // Override the auth type to ensure next login is using this auth type.
                            AuthType = Authtypes.EmailAndPassword;

                            // Report login result back to subscriber.
                            OnPlayFabLoginSuccess?.Invoke(result);
                        },

                        // Failure
                        (PlayFabError error) =>
                        {
                            //Report error result back to subscriber
                            OnPlayFabError?.Invoke(error);
                        });
                });
        }

        private void SilentlyAuthenticate(
            System.Action<LoginResult> successCallback = null, 
            System.Action<PlayFabError> errorCallback = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

            //Get the device id from native android
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            string deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");

            //Login with the android device ID
            PlayFabClientAPI.LoginWithAndroidDeviceID(
                new LoginWithAndroidDeviceIDRequest() 
                {
                    TitleId = PlayFabSettings.TitleId,
                    AndroidDevice = SystemInfo.deviceModel,
                    OS = SystemInfo.operatingSystem,
                    AndroidDeviceId = deviceId,
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams
                }, 
                
                (result) => 
                {            
            
                    PlayFabId = result.PlayFabId;
                    SessionTicket = result.SessionTicket;

                    OnPlayFabLoginSuccess?.Invoke(result);
                    successCallback?.Invoke(result);
                }, 
                
                (error) => 
                {

                    OnPlayFabError?.Invoke(error);
                    errorCallback?.Invoke(null);
                }
            );

#elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
            PlayFabClientAPI.LoginWithIOSDeviceID(
                new LoginWithIOSDeviceIDRequest() 
                {
                    TitleId = PlayFabSettings.TitleId,
                    DeviceModel = SystemInfo.deviceModel, 
                    OS = SystemInfo.operatingSystem,
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams

                }, 
                
                (result) => 
                {
                    PlayFabId = result.PlayFabId;
                    SessionTicket = result.SessionTicket;

                    OnPlayFabLoginSuccess?.Invoke(result);
                    successCallback?.Invoke(result);
                
                }, 
                
                (error) => 
                {

                    OnPlayFabError?.Invoke(error);
                    errorCallback?.Invoke(null);
                }
            );
#else
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = SystemInfo.deviceUniqueIdentifier,
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams
                }, 
                
                (result) => {

                    PlayFabId = result.PlayFabId;
                    SessionTicket = result.SessionTicket;

                    successCallback?.Invoke(result);
                    OnPlayFabLoginSuccess?.Invoke(result);

                }, 
                
                (error) => {

                    OnPlayFabError?.Invoke(error);
                    errorCallback?.Invoke(null);
                }
            );
#endif
        }

        private void UnlinkSilentAuth()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            //Get the device id from native android
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            string deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");

            //Fire and forget, unlink this android device.
            PlayFabClientAPI.UnlinkAndroidDeviceID(
                new UnlinkAndroidDeviceIDRequest() 
                {
                    AndroidDeviceId = deviceId
                }, 
                // success
                null, 
                // error
                null
            );

#elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
            PlayFabClientAPI.UnlinkIOSDeviceID(
                new UnlinkIOSDeviceIDRequest()
                {
                    DeviceId = SystemInfo.deviceUniqueIdentifier
                },
                // success
                null, 
                // error
                null
            );
#else
            PlayFabClientAPI.UnlinkCustomID(
                new UnlinkCustomIDRequest()
                {
                    CustomId = SystemInfo.deviceUniqueIdentifier
                },
                // success
                null,
                // error
                null
            );
#endif
        }

        private void OnSignInSuccessWithGoogle(GoogleSignInUser user)
        {
            _googleAuth.OnSignInSuccess -= OnSignInSuccessWithGoogle;
            UserDisplayName = user.DisplayName;

            PlayFabClientAPI.LoginWithGoogleAccount(
                new LoginWithGoogleAccountRequest()
                {
                    CreateAccount = true,
                    InfoRequestParameters = InfoRequestParams,
                    ServerAuthCode = user.AuthCode,
                },

                (result) =>
                {
                    LogUI.instance.AddStatusText("PlayFab login with google success!");
                    SetDisplayName(UserDisplayName);
                },

                (error) =>
                {
                    LogUI.instance.AddStatusText($"PlayFab error: {error.GenerateErrorReport()}");
                    OnPlayFabError?.Invoke(error);
                }
            );
        }

        private void OnSignInFailureWithGoogle(GoogleSignIn.SignInException error)
        {
            switch (error.Status)
            {
                default:
                    break;
                case GoogleSignInStatusCode.InvalidAccount:
                    OnPlayFabError?.Invoke(
                        new PlayFabError() 
                        {
                            Error = PlayFabErrorCode.InvalidAccount,
                        }
                    );
                    break;
            }
        }
    }
}