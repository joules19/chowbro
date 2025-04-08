namespace Chowbro.Core.Services.Interfaces.Auth;

public interface IDeviceValidationService
{
    Task<bool> IsValidDeviceAsync(string deviceId);
    Task UpdateLastSeenAsync(string deviceId);
}