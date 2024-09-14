using Google;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DoozyPractice.Bootstrap
{
    public class GoogleAuthentication : MonoBehaviour
    {
        [SerializeField]
        UnityEvent _signInSuccess;

        [SerializeField] 
        string _webClientId;

        [SerializeField]
        TMP_Text _debugText;


        private List<string> _messages = new();
        private GoogleSignInConfiguration _configuration;

        // Defer the configuration creation until Awake so the web Client ID
        // Can be set via the property inspector in the Editor.
        void Awake()
        {
            _configuration = new GoogleSignInConfiguration
            {
                WebClientId = _webClientId,
                RequestIdToken = true,
                RequestEmail = true,
            };
        }

        public void SignIn()
        {
            GoogleSignIn.Configuration = _configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;
            AddStatusText("Calling sign in google account!");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished, TaskScheduler.Default);
        }

        public void SignOut()
        {
            AddStatusText("Calling sign out google account!");
            Task.Run(() =>
            {
                GoogleSignIn.DefaultInstance.SignOut();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    AddStatusText("Sign out failed: " + task.Exception?.Message);
                }
                else
                {
                    AddStatusText("Sign out successful");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Disconnect()
        {
            AddStatusText("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        public void SignInSilently()
        {
            GoogleSignIn.Configuration = _configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            AddStatusText("Calling SignIn Silently");

            GoogleSignIn.DefaultInstance.SignInSilently()
                  .ContinueWith(OnAuthenticationFinished);
        }

        public void OnGamesSignIn()
        {
            GoogleSignIn.Configuration = _configuration;
            GoogleSignIn.Configuration.UseGameSignIn = true;
            GoogleSignIn.Configuration.RequestIdToken = false;

            AddStatusText("Calling Games SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
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
                    AddStatusText("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                }
            }
            else if (task.IsCanceled)
            {
                AddStatusText("Canceled");
            }
            else
            {
                AddStatusText("Welcome: " + task.Result.DisplayName + "!");
                _signInSuccess?.Invoke();
            }
        }
        void AddStatusText(string text)
        {
            if (_messages.Count == 5)
            {
                _messages.RemoveAt(0);
            }
            _messages.Add(text);
            string txt = "";
            foreach (string s in _messages)
            {
                txt += "\n" + s;
            }
            _debugText.text = txt;
        }
    }
}