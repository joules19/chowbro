using Chowbro.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using static Chowbro.Core.Enums.Product;
using static Chowbro.Modules.Vendors.DTOs.Product.ProductModel;

namespace Chowbro.Modules.Vendors.Commands.Product;

public class AddProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public Guid VendorId { get; set; }
    public Guid? BranchId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal BasePrice { get; set; }
    public Guid CategoryId { get; set; }
    public int PreparationTime { get; set; }
    public List<ProductOptionCategoryDto> OptionCategories { get; set; } = new();
    public List<IFormFile> Images { get; set; } = new();
}

public class ProductOptionCategoryDto
{
    public string Name { get; set; }
    public bool IsRequired { get; set; }
    public OptionSelectionType SelectionType { get; set; }
    public int? MinOptions { get; set; }
    public int? MaxOptions { get; set; }
    public List<ProductOptionDto> Options { get; set; } = new();
}

public class ProductOptionDto
{
    public string Name { get; set; }
    public decimal AdditionalPrice { get; set; }
}