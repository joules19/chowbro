

using Microsoft.AspNetCore.Identity;

namespace Chowbro.Core.Entities;
public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? RCNumber { get; set; } // Only for Vendors
    public string? ReferralCode { get; set; }

    private DateTime _dateOfBirth;
    public DateTime DateOfBirth
    {
        get => _dateOfBirth;
        set => _dateOfBirth = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public string? State { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public string? OtpCode { get; set; }

    private DateTime? _otpExpires;
    public DateTime? OtpExpires
    {
        get => _otpExpires;
        set => _otpExpires = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    // Refresh Token
    public string? RefreshToken { get; set; }

    private DateTime? _refreshTokenExpiryTime;
    public DateTime? RefreshTokenExpiryTime
    {
        get => _refreshTokenExpiryTime;
        set => _refreshTokenExpiryTime = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }
}
