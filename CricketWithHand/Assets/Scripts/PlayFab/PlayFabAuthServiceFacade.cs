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
        EmailAndPassword,
        GooglePlay
    }

    /// <summary>
    /// This class stores all the datas related to Authentication. 
    /// NOTE: Donot access this class directly.
    /// Use the instance of this class from PlayFabAuthServiceFacade for data reading.
    /// </summary>
    public class PlayFabAuthServiceData
    {
        #region Constants

        public const string _LoginRememberKey = "PlayFabLoginRemember";
        public const string _PlayFabRememberMeIdKey = "PlayFabIdPassGuid";
        public const string _PlayFabAuthTypeKey = "PlayFabAuthType";

        #endregion

        // These are fields that we set when we are using the service.
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthTicket { get; set; }
        public string PlayFabId { get; set; }
        public string SessionTicket { get; set; }
        public string UserDisplayName { get; set; }
        // This is a force link flag for custom ids for demoing
        public bool ForceLink { get; set; } = false;
        public GetPlayerCombinedInfoRequestParams InfoRequestParams { get; set; }
        public bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();

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
            set
            {
                PlayerPrefs.SetInt(_PlayFabAuthTypeKey, (int)value);
            }
        }

        /// <summary>
        /// Generated Remember Me ID
        /// Pass Null for a value to have one auto-generated.
        /// </summary>
        public string RememberMeId
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
    }

    public class PlayFabAuthServiceFacade
    {
        public bool IsLinkedWithGoogle => _googleAuth != null && _googleAuth.IsLoggedIn;

        public PlayFabAuthServiceData AuthData { get; private set; } = new();
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


        public void ClearCache()
        {
            AuthData.AuthType = Authtypes.None;

            PlayerPrefs.DeleteKey(PlayFabAuthServiceData._LoginRememberKey);
            PlayerPrefs.DeleteKey(PlayFabAuthServiceData._PlayFabRememberMeIdKey);
            PlayerPrefs.DeleteKey(PlayFabAuthServiceData._PlayFabAuthTypeKey);
        }


        /// <summary>
        /// Register a user with an Email & Password
        /// Note: We are not using the RegisterPlayFab API
        /// </summary>
        public void RegisterAccountWithEmailAndPassword(
            string email,
            string password,
            GetPlayerCombinedInfoRequestParams infoRequestParams,
            Action<AddUsernamePasswordResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            AuthData.AuthType = Authtypes.EmailAndPassword;
            AuthData.Email = email;
            AuthData.Password = password;
            AuthData.InfoRequestParams = infoRequestParams;

            // Any time we attempt to register a player, first silently authenticate the player.
            // This will retain the players True Origination (Android, iOS, Desktop)
            SilentlyAuthenticate(
                AuthData.InfoRequestParams,
                (LoginResult result) =>
                {
                    if (result == null)
                    {
                        //something went wrong with Silent Authentication, Check the debug console.
                        onFailure?.Invoke(new PlayFabError()
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
                            Username = AuthData.Username ?? result.PlayFabId, // Because it is required & Unique and not supplied by User.
                            Email = AuthData.Email,
                            Password = AuthData.Password,
                        },

                        // Success
                        (AddUsernamePasswordResult addResult) =>
                        {
                            AuthData.PlayFabId = result.PlayFabId;
                            AuthData.SessionTicket = result.SessionTicket;

                            // Need to think of this when we will configure Remember me
                            /*if (AuthData.RememberMe)
                            {
                                AuthData.RememberMeId = Guid.NewGuid().ToString();

                                PlayFabClientAPI.LinkCustomID(
                                    new LinkCustomIDRequest()
                                    {
                                        CustomId = AuthData.RememberMeId,
                                        ForceLink = AuthData.ForceLink
                                    },
                                    // success
                                    null,
                                    // error
                                    null
                                    );
                            }*/

                            AuthData.AuthType = Authtypes.EmailAndPassword;
                            onSuccess?.Invoke(addResult);
                        },

                        // Failure
                        (PlayFabError error) =>
                        {
                            onFailure?.Invoke(error);
                        }
                    );
                }
            );
        }

        /// <summary>
        /// Authenticate a user in PlayFab using an Email & Password combo
        /// </summary>
        public void AuthenticateEmailPassword(
            string email,
            string password,
            GetPlayerCombinedInfoRequestParams infoRequestParams,
            Action<LoginResult> OnSuccess = null, 
            Action<PlayFabError> OnFailure = null)
        {
            AuthData.AuthType = Authtypes.EmailAndPassword;
            AuthData.Email = email;
            AuthData.Password = password;
            AuthData.InfoRequestParams = infoRequestParams;

            // We have not opted for remember me in a previous session, so now we have to login the user with email & password.
            PlayFabClientAPI.LoginWithEmailAddress(
                new LoginWithEmailAddressRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    Email = AuthData.Email,
                    Password = AuthData.Password,
                    InfoRequestParameters = AuthData.InfoRequestParams
                },

                // Success
                (LoginResult result) =>
                {
                    AuthData.PlayFabId = result.PlayFabId;
                    AuthData.SessionTicket = result.SessionTicket;

                    // Note: At this point, they already have an account with PlayFab using a Username (email) & Password
                    // If RememberMe is checked, then generate a new Guid for Login with CustomId.
                    if (AuthData.RememberMe)
                    {
                        AuthData.RememberMeId = Guid.NewGuid().ToString();
                        AuthData.AuthType = Authtypes.EmailAndPassword;

                        // Fire and forget, but link a custom ID to this PlayFab Account.
                        PlayFabClientAPI.LinkCustomID(
                            new LinkCustomIDRequest
                            {
                                CustomId = AuthData.RememberMeId,
                                ForceLink = AuthData.ForceLink
                            },
                            null,   // Success callback
                            null    // Failure callback
                            );
                    }

                    OnSuccess?.Invoke(result);
                },

                // Failure
                (PlayFabError error) =>
                {
                    OnFailure?.Invoke(error);
                });
        }

        public void AuthenticateWithGoogle(
            string webClientId,
            GetPlayerCombinedInfoRequestParams infoRequestParams,
            Action<LoginResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            AuthData.InfoRequestParams = infoRequestParams;
            _googleAuth ??= new();
            _googleAuth.SignInAsync(
                webClientId, 
                (result) => OnSignInSuccessWithGoogle(result, onSuccess, onFailure), 
                (error) => OnSignInFailureWithGoogle(error, onFailure)
            );
        }

        public void LinkWithGoogle(
            string webClientId,
            GetPlayerCombinedInfoRequestParams infoRequestParams,
            Action<LinkGoogleAccountResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            AuthData.InfoRequestParams = infoRequestParams;
            _googleAuth ??= new();
            _googleAuth.SignInAsync(
                webClientId, 
                (result) => OnSignInSuccessWithGoogle_Link(result, onSuccess, onFailure),
                (error) => OnSignInFailureWithGoogle(error, onFailure)
            );
        }

        public void SetDisplayName(string displayName, 
            Action<string> OnSuccess = null,
            Action<PlayFabError> OnFailure = null)
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(
                new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = displayName
                },

                (UpdateUserTitleDisplayNameResult result) =>
                {
                    AuthData.UserDisplayName = result.DisplayName;
                    LogUI.instance.AddStatusText($"Display name set to: . {AuthData.UserDisplayName}");
                    OnSuccess?.Invoke(AuthData.UserDisplayName);
                },

                (PlayFabError error) =>
                {
                    LogUI.instance.AddStatusText($"{error.Error}, {error.GenerateErrorReport()}");
                    OnFailure?.Invoke(error);
                }
            );
        }

        public void LoginRememberedAccount(Action<LoginResult> onSuccess = null, Action<PlayFabError> onFailure = null)
        {
            //Check if the users has opted to be remembered.
            if (AuthData.RememberMe && !string.IsNullOrEmpty(AuthData.RememberMeId))
            {
                // If the user is being remembered, then log them in with a customid that was 
                // generated by the RememberMeId property
                PlayFabClientAPI.LoginWithCustomID(
                    new LoginWithCustomIDRequest()
                    {
                        TitleId = PlayFabSettings.TitleId,
                        CustomId = AuthData.RememberMeId,
                        CreateAccount = true,
                        InfoRequestParameters = AuthData.InfoRequestParams
                    },

                    // Success
                    (LoginResult result) =>
                    {
                        //Store identity and session
                        AuthData.PlayFabId = result.PlayFabId;
                        AuthData.SessionTicket = result.SessionTicket;

                        onSuccess?.Invoke(result);
                    },

                    // Failure
                    (PlayFabError error) =>
                    {
                        onFailure?.Invoke(error);
                    }
                );
            }
        }

        public void SilentlyAuthenticate(
            GetPlayerCombinedInfoRequestParams infoRequestParams,
            System.Action<LoginResult> successCallback = null,
            System.Action<PlayFabError> errorCallback = null)
        {
            AuthData.InfoRequestParams = infoRequestParams;

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
                    InfoRequestParameters = AuthData.InfoRequestParams
                }, 
                
                (result) => 
                {            
            
                    AuthData.AuthType = Authtypes.Silent;
                    AuthData.PlayFabId = result.PlayFabId;
                    AuthData.SessionTicket = result.SessionTicket;
                    successCallback?.Invoke(result);
                }, 
                
                (error) => 
                {
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
                    InfoRequestParameters = AuthData.InfoRequestParams

                }, 
                
                (result) => 
                {
                    AuthData.AuthType = Authtypes.Silent;
                    AuthData.PlayFabId = result.PlayFabId;
                    AuthData.SessionTicket = result.SessionTicket;
                    successCallback?.Invoke(result);
                
                }, 
                
                (error) => 
                {
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
                    InfoRequestParameters = AuthData.InfoRequestParams
                },

                (result) =>
                {
                    AuthData.AuthType = Authtypes.Silent;
                    AuthData.PlayFabId = result.PlayFabId;
                    AuthData.SessionTicket = result.SessionTicket;
                    successCallback?.Invoke(result);

                },

                (error) =>
                {
                    errorCallback?.Invoke(null);
                }
            );
#endif
        }

        public void LogOut()
        {
            if (!AuthData.IsLoggedIn)
            {
                LogUI.instance.AddStatusText("No user is logged in!");
                return;
            }

            switch (AuthData.AuthType)
            {
                case Authtypes.GooglePlay:
                    if (_googleAuth == null || !IsLinkedWithGoogle)
                    {
                        LogUI.instance.AddStatusText("Not logged in with Google account!");
                        return;
                    }
                        
                    _googleAuth.SignOut();
                    break;

                case Authtypes.Silent:
                    // UnlinkSilentAuth(); - We don't want to unlink the guest account from the device, as it should be permanent.
                    break;
                case Authtypes.None:
                    LogUI.instance.AddStatusText("THIS SHOULD NOT HAPPEN!");
                    break;
            }

            PlayFabClientAPI.ForgetAllCredentials();
            LogUI.instance.AddStatusText("Logged out of PlayFab!");
        }

        public void UnlinkSilentAuth(
            Action<UnlinkCustomIDResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            LogUI.instance.AddStatusText($"Unlinking device ID...!");

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

        private void OnSignInSuccessWithGoogle(GoogleSignInUser user, 
            Action<LoginResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            AuthData.UserDisplayName = user.DisplayName;

            PlayFabClientAPI.LoginWithGoogleAccount(
                new LoginWithGoogleAccountRequest()
                {
                    CreateAccount = true,
                    InfoRequestParameters = AuthData.InfoRequestParams,
                    ServerAuthCode = user.AuthCode,
                },

                (result) =>
                {
                    AuthData.AuthType = Authtypes.GooglePlay;
                    LogUI.instance.AddStatusText("PlayFab login with google success!");
                    onSuccess?.Invoke(result);

                    string username = result.InfoResultPayload.AccountInfo.Username;
                    if (username != null)
                    {
                        SetDisplayName(username);
                    }
                    else
                    {
                        SetDisplayName(AuthData.UserDisplayName);
                    }
                },

                (error) =>
                {
                    LogUI.instance.AddStatusText($"PlayFab error: {error.GenerateErrorReport()}");
                    onFailure?.Invoke(error);
                }
            );
        }

        private void OnSignInSuccessWithGoogle_Link(GoogleSignInUser user,
            Action<LinkGoogleAccountResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            AuthData.UserDisplayName = user.DisplayName;

            PlayFabClientAPI.LinkGoogleAccount(
                new LinkGoogleAccountRequest()
                {
                    ForceLink = false,
                    ServerAuthCode = user.AuthCode,
                },

                (result) =>
                {
                    AuthData.AuthType = Authtypes.GooglePlay;
                    LogUI.instance.AddStatusText("PlayFab linked with google success!");
                    onSuccess?.Invoke(result);
                },

                (error) =>
                {
                    LogUI.instance.AddStatusText($"PlayFab error: {error.GenerateErrorReport()}");
                    onFailure?.Invoke(error);
                }
            );
        }

        private void OnSignInFailureWithGoogle(Exception error,
            Action<PlayFabError> onFailure)
        {
            GoogleSignIn.SignInException googleError = error as GoogleSignIn.SignInException;

            switch (googleError.Status)
            {
                default:
                    break;

                case GoogleSignInStatusCode.InvalidAccount:
                    onFailure?.Invoke(
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