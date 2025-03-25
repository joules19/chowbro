using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chowbro.Infrastructure.Entities.Vendor;

namespace Chowbro.Infrastructure.Entities.Products
{
    public class Product : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();  

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? SKU { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Category { get; set; }

        // Foreign keys
        public Guid VendorId { get; set; }
        public Vendor.Vendor Vendor { get; set; }

        public Guid? BranchId { get; set; }
        public Branch Branch { get; set; }

        // Navigation properties
        public ICollection<ProductOptionCategory> OptionCategories { get; set; } = new List<ProductOptionCategory>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}