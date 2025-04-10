namespace Chowbro.Core.Events.Vendor
{
    public class VendorRegisteredEvent : UserRegisteredEvent
    {
        public string BusinessName { get; set; }
    }
}