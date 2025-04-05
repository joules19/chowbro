using Chowbro.Core.Interfaces.Rider;
using MediatR;

namespace Chowbro.Core.Events.Rider.Handlers
{
    public class RiderRegisteredEventHandler : INotificationHandler<RiderRegisteredEvent>
    {
        private readonly IRiderRepository _riderRepository;
        
        public RiderRegisteredEventHandler(IRiderRepository riderRepository)
        {
            _riderRepository = riderRepository;
        }
        
        public async Task Handle(RiderRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var rider = new Entities.Rider.Rider
            {
                UserId = notification.UserId,
                Email = notification.Email,
                PhoneNumber = notification.PhoneNumber,
                FirstName = notification.FirstName,
                LastName = notification.LastName,
                Status = Enums.Rider.RiderStatus.PendingVerification
            };
            
            await _riderRepository.AddAsync(rider, cancellationToken);
            await _riderRepository.SaveChangesAsync(cancellationToken);
        }
    }
}