using MediatR;

namespace Chowbro.Core.Events.Vendor
{
    public class VendorUpdatedEvent : INotification
    {
        public string UserId { get; }
        public string? FirstName { get; }
        public string? LastName { get; }

        public VendorUpdatedEvent(string userId, string? firstName, string? lastName)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}