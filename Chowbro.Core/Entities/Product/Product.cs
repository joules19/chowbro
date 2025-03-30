using Chowbro.Core.Entities;
using static Chowbro.Core.Enums.Product;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsAvailable { get; set; }
    public int PreparationTime { get; set; } // in minutes
    public ProductStatus Status { get; set; } = ProductStatus.Active;

    // Relationships
    public Guid VendorId { get; set; }
    public Vendor Vendor { get; set; }

    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; }

    public Guid CategoryId { get; set; }
    public ProductCategory Category { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductOptionCategory> OptionCategories { get; set; } = new List<ProductOptionCategory>();
}