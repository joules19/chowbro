using Chowbro.Core.Entities.Vendor;
using Chowbro.Core.Interfaces.Vendors;
using MediatR;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Core.Events.Handlers
{
    public class VendorRegisteredEventHandler : INotificationHandler<VendorRegisteredEvent>
    {
        private readonly IVendorRepository _vendorRepository;
        
        public VendorRegisteredEventHandler(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }
        
        public async Task Handle(VendorRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var vendor = new Vendor
            {
                BusinessName = notification.BusinessName,
                LastName = notification.LastName,
                FirstName = notification.FirstName,
                Email = notification.Email,
                PhoneNumber = notification.PhoneNumber,
                UserId = notification.UserId,
                Status = VendorStatus.PendingApproval,
                Description = "New vendor account"
            };
            
            await _vendorRepository.AddAsync(vendor);
            await _vendorRepository.SaveChangesAsync();
        }
    }
}