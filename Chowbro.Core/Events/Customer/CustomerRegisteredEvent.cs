// Chowbro.Core/Events/UserRegisteredEvent.cs
using MediatR;

namespace Chowbro.Core.Events
{
    public class UserRegisteredEvent : INotification
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }

    public class VendorRegisteredEvent : UserRegisteredEvent
    {
        public string BusinessName { get; set; }

    }

    public class CustomerRegisteredEvent : UserRegisteredEvent
    {
    }
    
    public class RiderRegisteredEvent : UserRegisteredEvent
    {
    }
}