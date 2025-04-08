using Chowbro.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Chowbro.Core.Events.Vendor.Handlers
{
    public class VendorUpdatedEventHandler : INotificationHandler<VendorUpdatedEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<VendorUpdatedEventHandler> _logger;

        public VendorUpdatedEventHandler(
            UserManager<ApplicationUser> userManager,
            ILogger<VendorUpdatedEventHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Handle(VendorUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(notification.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for vendor update event: {UserId}", notification.UserId);
                    return;
                }

                bool updated = false;
                if (notification.FirstName != null && user.FirstName != notification.FirstName)
                {
                    user.FirstName = notification.FirstName;
                    updated = true;
                }
                if (notification.LastName != null && user.LastName != notification.LastName)
                {
                    user.LastName = notification.LastName;
                    updated = true;
                }

                if (updated)
                {
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("Updated user details for vendor: {UserId}", notification.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling vendor updated event");
            }
        }
    }
}