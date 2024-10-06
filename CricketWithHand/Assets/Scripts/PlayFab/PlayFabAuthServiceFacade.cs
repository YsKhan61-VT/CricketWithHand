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
using CricketWithHand.Authentication.Google;
using Google;



namespace CricketWithHand.Authentication
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

        public const string REMEMBER_ME_TOGGLE_KEY = "RememberMe";
        public const string REMEMBER_ME_GUID_KEY = "RememberMeGuid";
        public const string AUTH_TYPE_KEY = "AuthenticationType";

        public const int PASSWORD_MIN_LENGTH = 6;
        public const int PASSWORD_MAX_LENGTH = 15;

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

        /// <summary>
        /// Remember the user next time they log in
        /// This is used for Auto-Login purpose.
        /// </summary>
        public bool RememberMe
        {
            get
            {
                return PlayerPrefs.GetInt(REMEMBER_ME_TOGGLE_KEY, 0) != 0;
            }
            set
            {
                PlayerPrefs.SetInt(REMEMBER_ME_TOGGLE_KEY, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Remember the type of authenticate for the user
        /// </summary>
        public Authtypes AuthType
        {
            get
            {
                return (Authtypes)PlayerPrefs.GetInt(AUTH_TYPE_KEY, 0);
            }
            set
            {
                PlayerPrefs.SetInt(AUTH_TYPE_KEY, (int)value);
            }
        }

        /// <summary>
        /// Generated Remember Me ID
        /// Pass Null for a value to have one auto-generated.
        /// </summary>
        public string CustomGUID
        {
            get
            {
                return PlayerPrefs.GetString(REMEMBER_ME_GUID_KEY, "");
            }
            set
            {
                var guid = value ?? Guid.NewGuid().ToString();
                PlayerPrefs.SetString(REMEMBER_ME_GUID_KEY, guid);
            }
        }
    }

    public class PlayFabAuthServiceFacade
    {
        public bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();
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

            PlayerPrefs.DeleteKey(PlayFabAuthServiceData.REMEMBER_ME_TOGGLE_KEY);
            PlayerPrefs.DeleteKey(PlayFabAuthServiceData.REMEMBER_ME_GUID_KEY);
            PlayerPrefs.DeleteKey(PlayFabAuthServiceData.AUTH_TYPE_KEY);
        }


        /// <summary>
        /// Register a user with an Email & Password
        /// Note: We are not using the RegisterPlayFab API
        /// </summary>
        public void RegisterWithEmailAndPassword(
            string email,
            string password,
            GetPlayerCombinedInfoRequestParams infoRequestParams,
            bool rememberMe,
            Action<LoginResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            AuthData.Email = email;
            AuthData.Password = password;
            AuthData.InfoRequestParams = infoRequestParams;
            AuthData.RememberMe = rememberMe;

            // Any time we attempt to register a player, first silently authenticate the player.
            // This will retain the players True Origination (Android, iOS, Desktop)
            SilentlyAuthenticate(
                AuthData.InfoRequestParams,
                (LoginResult result) =>
                {
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
                            AuthData.AuthType = Authtypes.EmailAndPassword;
                            AuthData.PlayFabId = result.PlayFabId;
                            AuthData.SessionTicket = result.SessionTicket;

                            // Need to think of this when we will configure Remember me
                            if (AuthData.RememberMe)
                            {
                                LinkWithCustomID();
                            }

                            onSuccess?.Invoke(result);
                        },

                        // Failure
                        (PlayFabError error) =>
                        {
                            // If can't add username, password, or email... then make sure to unlink the device.
                            UnlinkSilentAuth();
                            onFailure?.Invoke(error);
                        }
                    );
                },
                (error) =>
                {
                    //something went wrong with Silent Authentication, Check the debug console.
                    onFailure?.Invoke(error);
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
            bool rememberMe,
            Action<LoginResult> OnSuccess = null, 
            Action<PlayFabError> OnFailure = null)
        {
            AuthData.Email = email;
            AuthData.Password = password;
            AuthData.InfoRequestParams = infoRequestParams;
            AuthData.RememberMe = rememberMe;

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
                    AuthData.AuthType = Authtypes.EmailAndPassword;
                    AuthData.PlayFabId = result.PlayFabId;
                    AuthData.SessionTicket = result.SessionTicket;

                    // Note: At this point, they already have an account with PlayFab using a Username (email) & Password
                    // If RememberMe is checked, then generate a new Guid for Login with CustomId.
                    if (AuthData.RememberMe)
                    {
                        LinkWithCustomID();
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
            bool rememberMe,
            Action<LoginResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            AuthData.InfoRequestParams = infoRequestParams;
            AuthData.RememberMe = rememberMe;
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

        public void OnAuthenticationTimeOut()
        {
            // Force stop google authentication if possible
            _googleAuth?.SignOut();
            _googleAuth?.Disconnect();
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
                    OnSuccess?.Invoke(AuthData.UserDisplayName);
                },

                (PlayFabError error) =>
                {
                    OnFailure?.Invoke(error);
                }
            );
        }

        public void LoginRememberedAccount(Action<LoginResult> onSuccess = null, Action<PlayFabError> onFailure = null)
        {
            //Check if the users has opted to be remembered.
            if (AuthData.RememberMe && !string.IsNullOrEmpty(AuthData.CustomGUID))
            {
                // If the user is being remembered, then log them in with a customid that was 
                // generated by the RememberMeId property
                PlayFabClientAPI.LoginWithCustomID(
                    new LoginWithCustomIDRequest()
                    {
                        TitleId = PlayFabSettings.TitleId,
                        CustomId = AuthData.CustomGUID,
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

        public void LogOut(Action<string> onSuccess = null, Action<string> onFailure = null)
        {
            if (!IsLoggedIn)
            {
                onFailure?.Invoke("No user is logged in!");
                return;
            }

            switch (AuthData.AuthType)
            {
                case Authtypes.GooglePlay:
                    if (_googleAuth == null || !IsLinkedWithGoogle)
                    {
                        onFailure?.Invoke("Not logged in with Google account!");
                        return;
                    }
                        
                    _googleAuth.SignOut(
                        (result) => 
                        {
                            UnlinkSilentAuth();
                            PlayFabClientAPI.ForgetAllCredentials();
                            onSuccess?.Invoke($"{result}. Logged out of PlayFab!");
                        },
                        (error) => 
                        {
                            onFailure(error);
                            return;
                        }
                    );
                    break;

                case Authtypes.Silent:
                    UnlinkSilentAuth();
                    PlayFabClientAPI.ForgetAllCredentials();
                    onSuccess?.Invoke("Logged out of PlayFab!");
                    break;

                case Authtypes.EmailAndPassword:
                    UnlinkSilentAuth();
                    PlayFabClientAPI.ForgetAllCredentials();
                    onSuccess?.Invoke("Logged out of PlayFab!");
                    break;

                case Authtypes.None:
                    onFailure?.Invoke("THIS SHOULD NOT HAPPEN!");
                    break;
            }
        }

        public void UnlinkSilentAuth(
            Action<string> onSuccess = null,
            Action<PlayFabError> onFailure = null)
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
                (success) => 
                {
                    AuthData.AuthType = Authtypes.None;
                    onSuccess?.Invoke("Unlink android device successful!");
                },
                (error) => onFailure?.Invoke(error)
            );

#elif UNITY_IPHONE || UNITY_IOS && !UNITY_EDITOR
            PlayFabClientAPI.UnlinkIOSDeviceID(
                new UnlinkIOSDeviceIDRequest()
                {
                    DeviceId = SystemInfo.deviceUniqueIdentifier
                },
                (success) => 
                {
                    AuthData.AuthType = Authtypes.None;
                    onSuccess?.Invoke("Unlink IOS device successful!");
                },
                (error) => onFailure?.Invoke(error)
            );
#else
            PlayFabClientAPI.UnlinkCustomID(
                new UnlinkCustomIDRequest()
                {
                    CustomId = SystemInfo.deviceUniqueIdentifier
                },
                (success) =>
                {
                    AuthData.AuthType = Authtypes.None;
                    onSuccess?.Invoke("Unlink custom ID successful!");
                },
                (error) => onFailure?.Invoke(error)
            );
#endif
        }

        public bool IsPasswordValid(string password)
        {
            char[] chars = password.ToCharArray();
            return chars.Length >= PlayFabAuthServiceData.PASSWORD_MIN_LENGTH &&
                chars.Length <= PlayFabAuthServiceData.PASSWORD_MAX_LENGTH;
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

                    if (AuthData.RememberMe)
                    {
                        LinkWithCustomID();
                    }

                    onSuccess?.Invoke(result);
                },

                (error) =>
                {
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
                    onSuccess?.Invoke(result);
                },

                (error) =>
                {
                    onFailure?.Invoke(error);
                }
            );
        }

        private void OnSignInFailureWithGoogle(GoogleSignIn.SignInException error,
            Action<PlayFabError> onFailure)
        {
            switch (error.Status)
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

        private void LinkWithCustomID(
            Action<LinkCustomIDResult> onSuccess = null,
            Action<PlayFabError> onFailure = null)
        {
            AuthData.CustomGUID = Guid.NewGuid().ToString();

            PlayFabClientAPI.LinkCustomID(
                new LinkCustomIDRequest()
                {
                    CustomId = AuthData.CustomGUID,
                    ForceLink = AuthData.ForceLink
                },
                // success
                (result) => onSuccess?.Invoke(result),
                // error
                (error) => onFailure?.Invoke(error)
            );
        }
    }
}