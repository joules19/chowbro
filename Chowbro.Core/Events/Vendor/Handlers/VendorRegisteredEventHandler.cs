using Chowbro.Core.Interfaces.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Core.Events.Vendor.Handlers
{
    public class VendorRegisteredEventHandler : INotificationHandler<VendorRegisteredEvent>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ILogger<VendorRegisteredEventHandler> _logger;
        
        public VendorRegisteredEventHandler(
            IVendorRepository vendorRepository,
            ILogger<VendorRegisteredEventHandler> logger)
        {
            _vendorRepository = vendorRepository;
            _logger = logger;
        }
        
        public async Task Handle(VendorRegisteredEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Validate UserId
                if (string.IsNullOrWhiteSpace(notification.UserId))
                {
                    _logger.LogError("Vendor registration failed: Missing UserId for email {Email}", notification.Email);
                    return;
                }

                _logger.LogInformation("Processing vendor registration for UserId: {UserId}", notification.UserId);

                // Check if vendor already exists for this UserId
                var existingVendor = await _vendorRepository.GetByUserIdAsync(notification.UserId);
                if (existingVendor != null)
                {
                    _logger.LogWarning(
                        "Vendor already exists for UserId: {UserId} (VendorId: {VendorId}, Email: {Email})",
                        notification.UserId,
                        existingVendor.Id,
                        existingVendor.Email);
                    return;
                }

                // Create new vendor
                var vendor = new Entities.Vendor.Vendor
                {
                    BusinessName = notification.BusinessName ?? string.Empty,
                    LastName = notification.LastName,
                    FirstName = notification.FirstName,
                    Email = notification.Email,
                    PhoneNumber = notification.PhoneNumber,
                    UserId = notification.UserId,
                    Status = VendorStatus.PendingApproval,
                };
                
                await _vendorRepository.AddAsync(vendor);
                await _vendorRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Successfully registered new vendor {BusinessName} (UserId: {UserId}, VendorId: {VendorId})",
                    vendor.BusinessName,
                    vendor.UserId,
                    vendor.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vendor registration for UserId: {UserId}", notification.UserId);
                throw; // Re-throw to maintain the exception flow
            }
        }
    }
}