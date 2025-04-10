using Chowbro.Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chowbro.Infrastructure.Persistence.Repository.Auth
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly AppDbContext _context;

        public DeviceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Device device)
        {
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeviceExistsAsync(string deviceId)
        {
            return await _context.Devices
                .AnyAsync(d => d.DeviceId == deviceId);
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            return await _context.Devices.ToListAsync();
        }

        public async Task<Device> GetByIdAsync(Guid id)
        {
            return await _context.Devices.FindAsync(id);
        }

        public async Task<Device> GetByDeviceIdAsync(string deviceId)
        {
            return await _context.Devices
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        }

        public async Task<bool> IsDeviceActiveAsync(string deviceId)
        {
            var device = await GetByDeviceIdAsync(deviceId);
            return device?.IsActive ?? false;
        }

        public async Task<bool> IsDeviceBlacklistedAsync(string deviceId)
        {
            var device = await GetByDeviceIdAsync(deviceId);
            return device?.IsBlacklisted ?? false;
        }

        public async Task UpdateAsync(Device device)
        {
            _context.Devices.Update(device);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLastSeenAsync(string deviceId, DateTime timestamp)
        {
            var device = await GetByDeviceIdAsync(deviceId);
            if (device != null)
            {
                device.LastSeen = timestamp;
                await UpdateAsync(device);
            }
        }

        public async Task<IEnumerable<Device>> GetUserDevicesAsync(string userId)
        {
            return await _context.Devices
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        public async Task AssociateUserAsync(string deviceId, string userId)
        {
            var device = await GetByDeviceIdAsync(deviceId);
            if (device != null)
            {
                device.UserId = userId;
                await UpdateAsync(device);
            }
        }

        public async Task DisassociateUserAsync(string deviceId)
        {
            await AssociateUserAsync(deviceId, null);
        }

        public async Task<int> CountActiveDevicesAsync()
        {
            return await _context.Devices
                .CountAsync(d => d.IsActive);
        }

        public async Task<int> CountBlacklistedDevicesAsync()
        {
            return await _context.Devices
                .CountAsync(d => d.IsBlacklisted);
        }

        public async Task<IEnumerable<Device>> FindDevicesAsync(
            string searchTerm,
            int pageNumber,
            int pageSize)
        {
            return await _context.Devices
                .Where(d =>
                    d.DeviceId.Contains(searchTerm) ||
                    d.DeviceName.Contains(searchTerm) ||
                    d.Model.Contains(searchTerm))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetLoginCountToday(string deviceId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.DeviceLoginAttempts
                .Where(a => a.DeviceId == deviceId && a.AttemptDate >= today)
                .CountAsync();
        }

        public async Task FlagForReview(string deviceId)
        {
            var device = await GetByDeviceIdAsync(deviceId);
            if (device != null)
            {
                device.NeedsReview = true;
                device.ReviewReason = "Excessive login attempts";
                device.IsActive = false; // Temporarily deactivate
                await UpdateAsync(device);
            }
        }

        public async Task RecordLoginAttempt(string deviceId, string ipAddress)
        {
            var attempt = new DeviceLoginAttempt
            {
                DeviceId = deviceId,
                AttemptDate = DateTime.UtcNow,
                IpAddress = ipAddress
            };
            await _context.DeviceLoginAttempts.AddAsync(attempt);
            await _context.SaveChangesAsync();
        }

        public async Task AddAssociationHistoryAsync(DeviceAssociationHistory history)
        {
            await _context.DeviceAssociationHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DeviceAssociationHistory>> GetDeviceAssociationHistoryAsync(string deviceId)
        {
            return await _context.DeviceAssociationHistories
                .Where(h => h.DeviceId == deviceId)
                .OrderByDescending(h => h.AssociatedAt)
                .ToListAsync();
        }

        public async Task<List<DeviceAssociationHistory>> GetUserDeviceHistoryAsync(string userId)
        {
            return await _context.DeviceAssociationHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.AssociatedAt)
                .ToListAsync();
        }

        public async Task DisassociateDeviceAsync(string deviceId, string ipAddress = null)
        {
            var history = await _context.DeviceAssociationHistories
                .Where(h => h.DeviceId == deviceId && h.DisassociatedAt == null)
                .OrderByDescending(h => h.AssociatedAt)
                .FirstOrDefaultAsync();

            if (history != null)
            {
                history.DisassociatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            var device = await GetByDeviceIdAsync(deviceId);
            if (device != null)
            {
                device.UserId = null;
                await UpdateAsync(device);
            }
        }

        public async Task<DeviceAssociationHistory?> GetLastActivityAsync(string deviceId)
        {
            try
            {
                return await _context.DeviceAssociationHistories
                    .Where(d => d.DeviceId == deviceId)
                    .OrderByDescending(d => d.AssociatedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}