namespace Chowbro.Modules.Accounts.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}