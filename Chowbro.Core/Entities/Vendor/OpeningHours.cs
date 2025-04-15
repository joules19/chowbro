using System.ComponentModel.DataAnnotations;

namespace Chowbro.Core.Entities.Vendor
{
    public class OpeningHours : BaseEntity
    {

        public DayOfWeek Day { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsClosed { get; set; }

        public Guid StoreOperationId { get; set; }
        public StoreOperation StoreOperation { get; set; }
    }
}