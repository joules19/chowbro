
namespace Chowbro.Core.Events.Customer
{
    public class CustomerRegisteredEvent : UserRegisteredEvent
    {
        public string BusinessName { get; set; }

    }

}