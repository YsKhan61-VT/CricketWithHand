using System.Net.Mail;
using System;
using UnityEngine;
using System.Net;


namespace CricketWithHand.Authentication
{
    public class GmailServiceFacade : MonoBehaviour
    {
        [Serializable]
        public class SenderData
        {
            public string Email;               // your-email@example.com
            public string Name;                // Your Name
        }

        [Serializable]
        public class ReceiverData
        {
            public string Email;
            public string Name;
            public string Subject;
            public string Message;
        }

        [SerializeField]
        private SenderData _senderData;

        [SerializeField]
        private ReceiverData _receiverData;


        [ContextMenu("Send Email")]
        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_senderData.Email);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // Set to true if you're sending HTML

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(_senderData.Email, _senderData.Name),
                    EnableSsl = true
                };

                smtp.Send(mail);
                Debug.Log("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to send email: " + ex.Message);
            }
        }
    }
}

