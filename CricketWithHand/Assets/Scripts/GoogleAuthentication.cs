using Google;
using System;
using System.Threading;
using System.Threading.Tasks;
using YSK.Utilities;


namespace CricketWithHand.Authentication.Google
{
    public class GoogleAuthentication
    {
        public bool IsLoggedIn { get; private set; } = false;

        public async void SignInAsync(
            string webClientId, 
            Action<GoogleSignInUser> onSuccess = null, 
            Action<GoogleSignIn.SignInException> onFailure = null)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                RequestAuthCode = true,
                UseGameSignIn = false,
            };
            try
            {
                GoogleSignInUser result = await GoogleSignIn.DefaultInstance.SignIn();
                onSuccess?.Invoke(result);
                IsLoggedIn = true;
            }
            catch (GoogleSignIn.SignInException error)
            {
                onFailure?.Invoke(error);
            }
        }

        public async void SignInSilentlyAsync(
            string webClientId, 
            Action<GoogleSignInUser> onSuccess = null, 
            Action<Exception> onFailure = null)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = false
            };

            LogUI.instance.AddStatusText("Calling SignIn Silently");

            try
            {
                GoogleSignInUser user = await GoogleSignIn.DefaultInstance.SignInSilently();
                onSuccess?.Invoke(user);
                IsLoggedIn = true;
            }
            catch (Exception ex)
            {
                LogUI.instance.AddStatusText("Sign in failed: " + ex.Message);
                onFailure?.Invoke(ex);
            }
        }

        public async void SignInWithPlayGamesAsync(
            string webClientId, 
            Action<GoogleSignInUser> onSuccess = null, 
            Action<Exception> onFailure = null)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = true,
            };

            LogUI.instance.AddStatusText("Calling Games SignIn");

            try
            {
                GoogleSignInUser user = await GoogleSignIn.DefaultInstance.SignIn();
                onSuccess?.Invoke(user);
                IsLoggedIn = true;
            }
            catch (Exception ex)
            {
                LogUI.instance.AddStatusText("Sign in failed: " + ex.Message);
                onFailure?.Invoke(ex);
            }
        }

        public void SignOut(
            Action<string> onSuccess = null,
            Action<string> onFailure = null)
        {
            LogUI.instance.AddStatusText("Calling sign out google account!");
            Task.Run(() =>
            {
                GoogleSignIn.DefaultInstance.SignOut();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    onFailure?.Invoke($"Sign out failed: {task.Exception?.Message} !");
                }
                else
                {
                    // NOTE - not sure if need to call this or not.
                    Disconnect();

                    onSuccess?.Invoke("Sign out successful");
                    IsLoggedIn = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Disconnect()
        {
            LogUI.instance.AddStatusText("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }
    }
}