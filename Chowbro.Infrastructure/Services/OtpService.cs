using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Chowbro.Infrastructure.Services
{
    public interface IOtpService
    {
        Task SendOtpAsync(string contactInfo, string otp, bool isEmail = false);
    }

    public class OtpService : IOtpService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public OtpService(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;

            // Initialize Twilio only if needed
            TwilioClient.Init(_configuration["Twilio:AccountSid"], _configuration["Twilio:AuthToken"]);
        }

        public async Task SendOtpAsync(string contactInfo, string otp, bool isEmail = false)
        {
            bool useEmailForOtp = _configuration["OtpSettings:UseEmailForOtp"]?.ToLower() == "true";

            if (useEmailForOtp || isEmail)
            {
                // Send OTP via Email
                string subject = "Your Chowbro OTP Code";
                string body = $"Your Chowbro OTP is: <strong>{otp}</strong>. It expires in 5 minutes.";
                await _emailService.SendEmailAsync(contactInfo, subject, body);
            }
            else
            {
                // Send OTP via SMS (Twilio)
                await MessageResource.CreateAsync(
                    body: $"Your Chowbro OTP is: {otp}",
                    from: new PhoneNumber(_configuration["Twilio:PhoneNumber"]),
                    to: new PhoneNumber(contactInfo)
                );
            }
        }
    }
}