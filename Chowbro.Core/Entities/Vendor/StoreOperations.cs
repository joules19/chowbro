using System.ComponentModel.DataAnnotations;

namespace Chowbro.Core.Entities.Vendor
{
    public class StoreOperation : BaseEntity
    {
        public Guid VendorId { get; set; }
        public Vendor Vendor { get; set; }

        // Delivery Settings
        public DeliveryType DeliveryType { get; set; }
        public TimeSpan? OrderCutoffTime { get; set; }
        public TimeSpan? MenuReadyTime { get; set; }

        // Navigation property for opening hours
        public ICollection<OpeningHours> OpeningHours { get; set; } = new List<OpeningHours>();
    }

    public enum DeliveryType
    {
        Instant,
        PreOrder,
        InstantAndPreOrder
    }
}