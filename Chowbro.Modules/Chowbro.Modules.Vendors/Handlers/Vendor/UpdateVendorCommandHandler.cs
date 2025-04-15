using Chowbro.Core.Events.Vendor;
using Chowbro.Core.Models;
using Chowbro.Modules.Vendors.Commands.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Core.Services.Interfaces.Media;
using Chowbro.Core.Models.Vendor;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class UpdateVendorCommandHandler : IRequestHandler<UpdateVendorCommand, ApiResponse<object?>>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IBusinessTypeRepository _businessTypeRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateVendorCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;

        public UpdateVendorCommandHandler(
            IVendorRepository vendorRepository,
            IMediator mediator,
            ILogger<UpdateVendorCommandHandler> logger,
            ICurrentUserService currentUserService,
            ICloudinaryService cloudinaryService,
            IBusinessTypeRepository businessTypeRepository)
        {
            _vendorRepository = vendorRepository;
            _mediator = mediator;
            _logger = logger;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
            _businessTypeRepository = businessTypeRepository;
        }

        public async Task<ApiResponse<object?>> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vendorId = await _currentUserService.GetVendorIdAsync();
                if (!vendorId.HasValue)
                {
                    return ApiResponse<object?>.Fail(null, "Vendor not found", HttpStatusCode.NotFound);
                }

                var vendor = await _vendorRepository.GetByIdAsync(vendorId.Value, cancellationToken);
                if (vendor == null)
                {
                    return ApiResponse<object?>.Fail(null, "Vendor not found", HttpStatusCode.NotFound);
                }

                // Track changes for event publishing
                bool nameChanged = false;
                bool contactInfoChanged = false;
                bool logoChanged = false;
                bool coverChanged = false;

                // Update basic information
                if (request.BusinessName != null && vendor.BusinessName != request.BusinessName)
                {
                    vendor.BusinessName = request.BusinessName;
                }

                if (request.Description != null && vendor.Description != request.Description)
                {
                    vendor.Description = request.Description;
                }

                if (request.RcNumber != null && vendor.RcNumber != request.RcNumber)
                {
                    vendor.RcNumber = request.RcNumber;
                }

                // Update names and track changes
                if (request.FirstName != null && vendor.FirstName != request.FirstName)
                {
                    vendor.FirstName = request.FirstName;
                    nameChanged = true;
                }

                if (request.LastName != null && vendor.LastName != request.LastName)
                {
                    vendor.LastName = request.LastName;
                    nameChanged = true;
                }

                // Update contact information
                if (request.Email != null && vendor.Email != request.Email)
                {
                    vendor.Email = request.Email;
                    contactInfoChanged = true;
                }

                if (request.PhoneNumber != null && vendor.PhoneNumber != request.PhoneNumber)
                {
                    vendor.PhoneNumber = request.PhoneNumber;
                    contactInfoChanged = true;
                }

                if (request.BusinessEmail != null && vendor.BusinessEmail != request.BusinessEmail)
                {
                    vendor.BusinessEmail = request.BusinessEmail;
                    contactInfoChanged = true;
                }

                if (request.BusinessPhoneNumber != null && vendor.BusinessPhoneNumber != request.BusinessPhoneNumber)
                {
                    vendor.BusinessPhoneNumber = request.BusinessPhoneNumber;
                    contactInfoChanged = true;
                }

                // Update business type if provided
                if (request.BusinessTypeId != null && vendor.BusinessTypeId != request.BusinessTypeId)
                {
                    var businessTypeExists = await _businessTypeRepository.ExistsAsync(request.BusinessTypeId.Value, cancellationToken);
                    if (!businessTypeExists)
                    {
                        return ApiResponse<object?>.Fail(null, "Invalid Business Type ID", HttpStatusCode.BadRequest);
                    }

                    vendor.BusinessTypeId = request.BusinessTypeId;
                }

                // Handle logo upload
                if (request.LogoFile != null)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(request.LogoFile);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Failed to upload logo: {Error}", uploadResult.Error.Message);
                        return ApiResponse<object?>.Fail(null, "Failed to upload logo", HttpStatusCode.BadRequest);
                    }

                    // Delete old logo if exists
                    if (!string.IsNullOrEmpty(vendor.LogoPublicId))
                    {
                        await _cloudinaryService.DeleteImageAsync(vendor.LogoPublicId);
                    }

                    vendor.LogoUrl = uploadResult.SecureUrl.ToString();
                    vendor.LogoPublicId = uploadResult.PublicId;
                    logoChanged = true;
                }

                // Handle cover image upload
                if (request.CoverImageFile != null)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(request.CoverImageFile);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Failed to upload cover image: {Error}", uploadResult.Error.Message);
                        return ApiResponse<object?>.Fail(null, "Failed to upload cover image", HttpStatusCode.BadRequest);
                    }

                    // Delete old cover if exists
                    if (!string.IsNullOrEmpty(vendor.CoverPublicId))
                    {
                        await _cloudinaryService.DeleteImageAsync(vendor.CoverPublicId);
                    }

                    vendor.CoverImageUrl = uploadResult.SecureUrl.ToString();
                    vendor.CoverPublicId = uploadResult.PublicId;
                    coverChanged = true;
                }

                // Save all changes
                await _vendorRepository.UpdateAsync(vendor, cancellationToken);

                // Publish event to update Application User if needed
                if (nameChanged)
                {
                    await _mediator.Publish(new VendorUpdatedEvent(
                        vendor.UserId,
                        vendor.FirstName,
                        vendor.LastName), cancellationToken);
                }

                var vendorDto = new VendorDto
                {
                    Id = vendor.Id,
                    BusinessName = vendor.BusinessName,
                    FirstName = vendor.FirstName,
                    LastName = vendor.LastName,
                    RcNumber = vendor.RcNumber,
                    Description = vendor.Description,
                    LogoUrl = vendor.LogoUrl,
                    CoverImageUrl = vendor.CoverImageUrl,
                    CoverPublicId = vendor.CoverPublicId,
                    LogoPublicId = vendor.LogoPublicId,
                    PhoneNumber = vendor.PhoneNumber,
                    Email = vendor.Email,
                    BusinessPhoneNumber = vendor.BusinessPhoneNumber,
                    BusinessEmail = vendor.BusinessEmail,
                    UserId = vendor.UserId,
                    BusinessTypeId = vendor.BusinessTypeId,
                    BusinessTypeName = vendor.BusinessType?.Name,
                    Status = vendor.Status.ToString(),
                    CreatedAt = vendor.CreatedAt,
                    Branches = vendor.Branches.Select(b => new BranchDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        PhoneNumber = b.PhoneNumber,
                        Address = b.Address,
                        City = b.City,
                        StateId = b.StateId ?? Guid.Empty,
                        LgaId = b.LgaId ?? Guid.Empty,
                        Country = b.Country,
                        PostalCode = b.PostalCode,
                        Latitude = b.Latitude,
                        Longitude = b.Longitude,
                        IsMainBranch = b.IsMainBranch
                    }).ToList()
                };

                return ApiResponse<object?>.Success(vendorDto, "Vendor updated successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vendor");
                return ApiResponse<object?>.Fail(null, "Error updating vendor", HttpStatusCode.InternalServerError);
            }
        }
    }
}