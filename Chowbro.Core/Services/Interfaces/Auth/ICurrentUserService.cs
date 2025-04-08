namespace Chowbro.Core.Services.Interfaces.Auth;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? PhoneNumber { get; }
    string? Name { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsVendor { get; }
    Task<Guid?> GetVendorIdAsync();
    Task<Guid> GetRequiredVendorIdAsync();
}