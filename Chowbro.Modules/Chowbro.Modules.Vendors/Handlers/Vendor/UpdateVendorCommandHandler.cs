using Chowbro.Core.Entities.Vendor;
using Chowbro.Core.Events.Vendor;
using Chowbro.Core.Interfaces.Vendor;
using Chowbro.Core.Interfaces.Media;
using Chowbro.Core.Interfaces.Auth;
using Chowbro.Core.Models;
using Chowbro.Modules.Vendors.Commands.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class UpdateVendorCommandHandler : IRequestHandler<UpdateVendorCommand, ApiResponse<bool>>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateVendorCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;

        public UpdateVendorCommandHandler(
            IVendorRepository vendorRepository,
            IMediator mediator,
            ILogger<UpdateVendorCommandHandler> logger,
            ICurrentUserService currentUserService,
            ICloudinaryService cloudinaryService)
        {
            _vendorRepository = vendorRepository;
            _mediator = mediator;
            _logger = logger;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vendorId = await _currentUserService.GetVendorIdAsync();
                if (!vendorId.HasValue)
                {
                    return ApiResponse<bool>.Fail(false, "Vendor not found", HttpStatusCode.NotFound);
                }

                var vendor = await _vendorRepository.GetByIdAsync(vendorId.Value, cancellationToken);
                if (vendor == null)
                {
                    return ApiResponse<bool>.Fail(false, "Vendor not found", HttpStatusCode.NotFound);
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

                // Handle logo upload
                if (request.LogoFile != null)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(request.LogoFile);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Failed to upload logo: {Error}", uploadResult.Error.Message);
                        return ApiResponse<bool>.Fail(false, "Failed to upload logo", HttpStatusCode.BadRequest);
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
                        return ApiResponse<bool>.Fail(false, "Failed to upload cover image", HttpStatusCode.BadRequest);
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

                return ApiResponse<bool>.Success(true, "Vendor updated successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vendor");
                return ApiResponse<bool>.Fail(false, "Error updating vendor", HttpStatusCode.InternalServerError);
            }
        }
    }
}