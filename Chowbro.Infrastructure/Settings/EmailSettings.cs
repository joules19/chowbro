namespace Chowbro.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string DisplayName { get; set; } = string.Empty;
        public bool EnableVerification { get; set; }
        public string From { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string VerificationBaseUrl { get; set; } = string.Empty;
    }
}