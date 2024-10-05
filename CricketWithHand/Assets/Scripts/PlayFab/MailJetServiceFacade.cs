using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;
using UnityEngine;

namespace CricketWithHand.Authentication
{
    public class MailJetServiceFacade : MonoBehaviour
    {
        [Serializable]
        public class SenderData
        {
            public string ApiKey;                   // YOUR_API_KEY
            public string ApiSecret;                // YOUR_API_SECRET
            public string AdminEmail;               // your-email@example.com
            public string AdminName;                // Your Name
            public string ApiUrl;                   // https://api.mailjet.com/v3/send
        }

        public class ReceiverData
        {
            public string Email;
            public string Name;
            public string Subject;
            public string Message;
        }

        private readonly HttpClient _httpClient = new HttpClient();

        [SerializeField]
        private SenderData _initData;

        public async void SendEmailAsync(
            ReceiverData receiverData,
            Action onSendSuccess,
            Action<string> onSendFailed)
        {
            // Create the request content
            var content = new
            {
                Messages = new[]
                {
                    new
                    {
                        From = new { Email = _initData.AdminEmail, Name = _initData.AdminName },
                        To = new[] 
                        { 
                            new 
                            {
                                Email = receiverData.Email,
                                Name = receiverData.Name
                            } 
                        },
                         receiverData.Subject,
                        TextPart = receiverData.Message,
                        HTMLPart = $"<h3>{receiverData.Message}</h3>"
                    }
                }
            };

            var jsonContent = new StringContent(JsonUtility.ToJson(content), Encoding.UTF8, "application/json");

            // Set up Basic Authentication
            var byteArray = Encoding.ASCII.GetBytes($"{_initData.ApiKey}:{_initData.ApiSecret}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                // Send the request
                var response = await _httpClient.PostAsync(_initData.ApiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    onSendSuccess?.Invoke();
                }
                else
                {
                    onSendFailed?.Invoke(response.Content.ToString());
                }
            }
            catch (Exception ex)
            {
                onSendFailed?.Invoke(ex.ToString());
            }
        }
    }
}