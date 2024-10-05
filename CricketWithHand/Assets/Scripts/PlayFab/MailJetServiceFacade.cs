using System.Net.Http;
using System.Text;
using System;
using UnityEngine;
using Newtonsoft.Json;

namespace CricketWithHand.Authentication
{
    public class MailJetServiceFacade : MonoBehaviour
    {
        [Serializable]
        public class SenderData
        {
            public string ApiKey;                   // YOUR_API_KEY
            public string ApiSecret;                // YOUR_API_SECRET
            public string SenderEmail;               // your-email@example.com
            public string SenderName;                // Your Name
            public string Endpoint;                 // https://api.mailjet.com/v3.1/send
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
                var emailData = new
                {
                    Messages = new[]
                    {
                        new
                        {
                            From = new { Email = _initData.SenderEmail, Name = _initData.SenderName },
                            To = new[] { new { Email = receiverData.Email, Name = "User" } },
                            Subject = receiverData.Subject,
                            TextPart = receiverData.Message,
                            HTMLPart = "<h3>" + receiverData.Message + "</h3>"  // Optional, can add HTML part if needed
                        }
                    }
                };

            // Convert email data to JSON using Newtonsoft.Json
            string json = JsonConvert.SerializeObject(emailData, Formatting.Indented);

            Debug.Log($"JSON Payload: {json}"); // Log the JSON payload for debugging

            // Set the API credentials
            var byteArray = Encoding.ASCII.GetBytes($"{_initData.ApiKey}:{_initData.ApiSecret}");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // Create the StringContent
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Send the POST request
                HttpResponseMessage response = await _httpClient.PostAsync(_initData.Endpoint, content);

                // Check for success status code
                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Email sent successfully!");
                    onSendSuccess?.Invoke();
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.LogError($"Error: {response.StatusCode} - {errorResponse}");
                    onSendFailed?.Invoke(errorResponse);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception occurred: {ex.Message}");
                onSendFailed?.Invoke(ex.Message);
            }
        }
    }
}


/*using var client = new HttpClient { BaseAddress = new Uri("https://api.mailjet.com/v3/") };

            // Set the authorization header
            var byteArray = Encoding.UTF8.GetBytes($"{_initData.ApiKey} : {_initData.ApiSecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var emailData = new
            {
                Messages = new[]
                {
                    new
                    {
                        From = new { Email = _initData.AdminEmail, Name = _initData.AdminName },
                        To = new[] { new { Email = receiverData.Email} },
                        Subject = receiverData.Subject,
                        TextPart = receiverData.Message,
                        HTMLPart = $"<h3>{receiverData.Message}</h3>"
                    }
                }
            };

            // Serialize the emailData object to JSON using Newtonsoft.Json
            var json = JsonConvert.SerializeObject(emailData);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Send the request
                var response = await client.PostAsync("send", content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Email sent successfully!");
                    onSendSuccess?.Invoke();
                }
                else
                {
                    // Read the response content for more details on the error
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error sending email: {response.StatusCode}");
                    Console.WriteLine($"Response content: {responseContent}");
                    onSendFailed?.Invoke($"Code: {response.StatusCode}, Content: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                onSendFailed?.Invoke(ex.Message);
            }*/