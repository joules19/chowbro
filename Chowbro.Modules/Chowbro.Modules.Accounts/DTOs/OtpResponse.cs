namespace Chowbro.Modules.Accounts.DTOs;

public class OtpResponse
{
    public string? Message { get; set; } = string.Empty;
    public bool? RequiresOtp { get; set; } = false;
}
