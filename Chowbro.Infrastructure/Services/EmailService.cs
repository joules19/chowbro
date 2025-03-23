using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using Chowbro.Infrastructure.Settings;

namespace Chowbro.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.From));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;

                email.Body = new TextPart("html") { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}