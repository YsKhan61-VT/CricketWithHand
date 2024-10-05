using System.Net.Http;
using System.Text;
using System;
using UnityEngine;
using Newtonsoft.Json;


namespace CricketWithHand.Authentication
{
    public class MailJetServiceFacade
    {
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
        private SenderData _senderData;

        public MailJetServiceFacade()
        {
            _senderData = new SenderData()
            {
                ApiKey = "d698093746aee705afcb9b20cbc17f6d",
                ApiSecret = "9a2fab9d22c8a1d23289731a21b9c518",
                SenderEmail = "yskhan61@gmail.com",
                SenderName = "CricketWithHand",
                Endpoint = "https://api.mailjet.com/v3.1/send"
            };
        }


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
                            From = new { Email = _senderData.SenderEmail, Name = _senderData.SenderName },
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
            var byteArray = Encoding.ASCII.GetBytes($"{_senderData.ApiKey}:{_senderData.ApiSecret}");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // Create the StringContent
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Send the POST request
                HttpResponseMessage response = await _httpClient.PostAsync(_senderData.Endpoint, content);

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
