using Chowbro.Core.Entities.Vendor;

namespace Chowbro.Core.Entities.Vendor
{
    public class BusinessType : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation property
        public ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
    }
}