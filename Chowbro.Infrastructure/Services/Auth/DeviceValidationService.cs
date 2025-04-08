using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Infrastructure.Persistence.Repository.Auth;

namespace Chowbro.Infrastructure.Services.Auth;


    public class DeviceValidationService : IDeviceValidationService
    {
        private readonly IDeviceRepository _deviceRepository;

        public DeviceValidationService(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public async Task<bool> IsValidDeviceAsync(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return false;
            }

            var exists = await _deviceRepository.DeviceExistsAsync(deviceId);
            var isActive = await _deviceRepository.IsDeviceActiveAsync(deviceId);
            var isBlacklisted = await _deviceRepository.IsDeviceBlacklistedAsync(deviceId);

            return exists && isActive && !isBlacklisted;
        }

        public async Task UpdateLastSeenAsync(string deviceId)
        {
            await _deviceRepository.UpdateLastSeenAsync(deviceId, DateTime.UtcNow);
        }
    }
