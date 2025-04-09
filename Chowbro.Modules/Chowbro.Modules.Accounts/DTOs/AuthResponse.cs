namespace Chowbro.Modules.Accounts.DTOs
{
    public class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? DeviceId { get; set; }

        public DateTime? Expiration { get; set; }
        public UserDto? User { get; set; }
    }
}