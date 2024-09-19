using Google;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YSK.Utilities;

namespace DoozyPractice.PlayFab.Google
{
    public class GoogleAuthentication
    {
        public event Action<GoogleSignInUser> OnSignInSuccess;

        public bool IsLoggedIn { get; private set; } = false;

        public void SignInWithGoogle(string webClientId)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                RequestAuthCode = true,
                UseGameSignIn = false,
            };

            LogUI.instance.AddStatusText("Calling sign in google account!");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished, TaskScheduler.Default);
        }

        public void SignInSilently(string webClientId)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = false
            };

            LogUI.instance.AddStatusText("Calling SignIn Silently");

            GoogleSignIn.DefaultInstance.SignInSilently()
                  .ContinueWith(OnAuthenticationFinished);
        }

        public void SignInWithPlayGames(string webClientId)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = true,
            };

            LogUI.instance.AddStatusText("Calling Games SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
        }

        public void SignOut()
        {
            LogUI.instance.AddStatusText("Calling sign out google account!");
            Task.Run(() =>
            {
                GoogleSignIn.DefaultInstance.SignOut();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    LogUI.instance.AddStatusText("Sign out failed: " + task.Exception?.Message);
                }
                else
                {
                    LogUI.instance.AddStatusText("Sign out successful");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Disconnect()
        {
            LogUI.instance.AddStatusText("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using IEnumerator<System.Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    LogUI.instance.AddStatusText("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    LogUI.instance.AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                }
            }
            else if (task.IsCanceled)
            {
                LogUI.instance.AddStatusText("Canceled");
            }
            else
            {
                LogUI.instance.AddStatusText("Welcome: " + task.Result.DisplayName + "!");
                IsLoggedIn = true;
                OnSignInSuccess?.Invoke(task.Result);
            }
        }
    }
}