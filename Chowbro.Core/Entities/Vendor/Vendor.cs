using Chowbro.Core.Entities;
using static Chowbro.Core.Enums.Vendor;

public class Vendor : BaseEntity
{
    public string Name { get; set; }
    public string RcNumber { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
    public string CoverImageUrl { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public VendorStatus Status { get; set; } = VendorStatus.PendingApproval;

    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

}