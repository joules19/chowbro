using Chowbro.Core.Entities.Auth;

namespace Chowbro.Infrastructure.Persistence.Repository.Auth
{
    public interface IDeviceRepository
    {
        // Basic CRUD
        Task<Device> GetByIdAsync(Guid id);
        Task<Device> GetByDeviceIdAsync(string deviceId);
        Task<IEnumerable<Device>> GetAllAsync();
        Task AddAsync(Device device);
        Task UpdateAsync(Device device);
        Task DeleteAsync(Guid id);

        // Validation & Status
        Task<bool> DeviceExistsAsync(string deviceId);
        Task<bool> IsDeviceActiveAsync(string deviceId);
        Task<bool> IsDeviceBlacklistedAsync(string deviceId);
        Task UpdateLastSeenAsync(string deviceId, DateTime timestamp);

        // User-related
        Task<IEnumerable<Device>> GetUserDevicesAsync(string userId);
        Task AssociateUserAsync(string deviceId, string userId);
        Task DisassociateUserAsync(string deviceId);

        // Bulk operations
        Task<int> CountActiveDevicesAsync();
        Task<int> CountBlacklistedDevicesAsync();

        // Advanced queries
        Task<IEnumerable<Device>> FindDevicesAsync(
            string searchTerm,
            int pageNumber,
            int pageSize);

        Task<int> GetLoginCountToday(string deviceId);
        Task FlagForReview(string deviceId);
        Task RecordLoginAttempt(string deviceId, string ipAddress);

        Task AddAssociationHistoryAsync(DeviceAssociationHistory history);
        Task<List<DeviceAssociationHistory>> GetDeviceAssociationHistoryAsync(string deviceId);
        Task<List<DeviceAssociationHistory>> GetUserDeviceHistoryAsync(string userId);
        Task DisassociateDeviceAsync(string deviceId, string ipAddress = null);
        Task<DeviceAssociationHistory?> GetLastActivityAsync(string deviceId);

    }
}