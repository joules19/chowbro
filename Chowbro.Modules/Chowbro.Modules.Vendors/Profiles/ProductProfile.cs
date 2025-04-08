using AutoMapper;
using Chowbro.Core.Entities.Product;
using Chowbro.Modules.Vendors.Commands.Product;
using static Chowbro.Modules.Vendors.DTOs.Product.ProductModel;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<ProductOptionCategory, ProductOptionCategoryDto>();
        CreateMap<ProductOption, ProductOptionDto>();
    }
}