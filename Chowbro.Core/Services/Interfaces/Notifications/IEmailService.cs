namespace Chowbro.Core.Services.Interfaces.Notifications
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}