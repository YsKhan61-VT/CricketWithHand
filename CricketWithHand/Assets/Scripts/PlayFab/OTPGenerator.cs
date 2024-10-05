using System;


namespace CricketWithHand.Authentication
{
    public class OTPGenerator
    {
        private const int WAIT_FOR_MINUTES_BEFORE_ALLOW_OTP_RESEND = 2;
        private const int OTP_LENGTH = 6;

        private TimeSpan _resendDelay = TimeSpan.FromMinutes(WAIT_FOR_MINUTES_BEFORE_ALLOW_OTP_RESEND);

        public string GeneratedOTP { get; private set; }

        private DateTime _otpSentTime;

        public OTPGenerator()
        {
            GeneratedOTP = string.Empty;
        }

        public string GenerateOTP()
        {
            _otpSentTime = DateTime.Now;

            // Generate a random 6-digit OTP
            GeneratedOTP = "";
            for (int i = 0; i < OTP_LENGTH; i++)
            {
                GeneratedOTP += UnityEngine.Random.Range(0, 9).ToString();
            }
            return GeneratedOTP;
        }

        public bool CanRegenerateOTP()
        {
            // If this is the first time, an otp gonna be generated, then do it instantly.
            if (string.IsNullOrEmpty(GeneratedOTP)) return true;

            // Calculate the time difference between the current time and the time OTP was sent
            TimeSpan timeElapsed = DateTime.Now - _otpSentTime;

            // Return true if enough time has passed, otherwise false
            return timeElapsed >= _resendDelay;
        }

        public int RemainingSecondsToRegenerateOTP()
        {
            TimeSpan timeRemaining = _resendDelay - (DateTime.Now - _otpSentTime);
            return timeRemaining.Seconds;
        }
    }
}
