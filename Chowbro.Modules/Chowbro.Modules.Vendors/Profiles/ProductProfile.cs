using AutoMapper;
using Chowbro.Modules.Vendors.Commands;
using static Chowbro.Modules.Vendors.DTOs.ProductModel;

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