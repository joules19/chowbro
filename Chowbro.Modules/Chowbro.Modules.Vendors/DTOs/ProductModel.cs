using Chowbro.Modules.Vendors.Commands;

namespace Chowbro.Modules.Vendors.DTOs;

public class ProductModel
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsAvailable { get; set; }
        public int PreparationTime { get; set; }
        public Guid VendorId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public List<ProductOptionCategoryDto> OptionCategories { get; set; } = new();
    }

    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }

}