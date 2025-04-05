using Chowbro.Core.Entities.Customer;
using Chowbro.Core.Interfaces.Customer;
using MediatR;

namespace Chowbro.Core.Events.Handlers
{
    public class CustomerRegisteredEventHandler : INotificationHandler<CustomerRegisteredEvent>
    {
        private readonly ICustomerRepository _customerRepository;
        
        public CustomerRegisteredEventHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        
        public async Task Handle(CustomerRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                UserId = notification.UserId,
                Email = notification.Email,
                PhoneNumber = notification.PhoneNumber,
                FirstName = notification.FirstName,
                LastName = notification.LastName
            };
            
            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();
        }
    }
}