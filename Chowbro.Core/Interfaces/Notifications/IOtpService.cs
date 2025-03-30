namespace Chowbro.Core.Interfaces.Notifications
{
    public interface IOtpService
    {
        Task SendOtpAsync(string contactInfo, string otp, bool isEmail = false);
    }
}
