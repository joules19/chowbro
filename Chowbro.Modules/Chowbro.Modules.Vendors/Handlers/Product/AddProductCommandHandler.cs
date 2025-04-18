﻿
using AutoMapper;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Product;
using Chowbro.Modules.Vendors.Commands.Product;
using MediatR;
using System.Net;
using Chowbro.Core.Entities.Product;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Core.Services.Interfaces.Media;
using static Chowbro.Modules.Vendors.DTOs.Product.ProductModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, ApiResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public AddProductCommandHandler(
        IProductRepository productRepository,
        IProductCategoryRepository categoryRepository,
        ICloudinaryService cloudinaryService,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _cloudinaryService = cloudinaryService;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ProductDto>> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {

        var vendorId = await _currentUserService.GetVendorIdAsync();
        if (vendorId == null)
        {
            return ApiResponse<ProductDto>.Fail(
                null,
                "Vendor account not properly configured",
                HttpStatusCode.Forbidden
            );
        }
        request.VendorId = vendorId.Value;

        // Validate vendor owns the category
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null || category.VendorId != request.VendorId)
        {
            return ApiResponse<ProductDto>.Fail(null, "Invalid product category", HttpStatusCode.NotFound);
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            BasePrice = request.BasePrice,
            CategoryId = request.CategoryId,
            VendorId = request.VendorId,
            BranchId = request.BranchId,
            PreparationTime = request.PreparationTime,
            IsAvailable = true
        };

        // Add option categories
        foreach (var optionCategoryDto in request.OptionCategories)
        {
            var optionCategory = new ProductOptionCategory
            {
                Name = optionCategoryDto.Name,
                IsRequired = optionCategoryDto.IsRequired,
                SelectionType = optionCategoryDto.SelectionType,
                MinOptions = optionCategoryDto.MinOptions,
                MaxOptions = optionCategoryDto.MaxOptions,
                Product = product
            };

            foreach (var optionDto in optionCategoryDto.Options)
            {
                optionCategory.Options.Add(new ProductOption
                {
                    Name = optionDto.Name,
                    AdditionalPrice = optionDto.AdditionalPrice,
                    IsAvailable = true
                });
            }

            product.OptionCategories.Add(optionCategory);
        }

        // Upload images
        foreach (var imageFile in request.Images)
        {
            var uploadResult = await _cloudinaryService.UploadImageAsync(imageFile);
            if (uploadResult.Error != null)
            {
                continue; // or handle error
            }

            product.Images.Add(new ProductImage
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                IsMain = product.Images.Count == 0 // First image is main
            });
        }

        await _productRepository.AddAsync(product);

        return ApiResponse<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}