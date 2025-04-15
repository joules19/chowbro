using Chowbro.Core.Models;
using Chowbro.Modules.Vendors.Commands.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Core.Services.Interfaces.Media;
using Chowbro.Core.Models.Vendor;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class UpdateVendorStatusCommandHandler : IRequestHandler<UpdateVendorStatusCommand, ApiResponse<object?>>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IBusinessTypeRepository _businessTypeRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateVendorStatusCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;

        public UpdateVendorStatusCommandHandler(
            IVendorRepository vendorRepository,
            IMediator mediator,
            ILogger<UpdateVendorStatusCommandHandler> logger,
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

        public async Task<ApiResponse<object?>> Handle(UpdateVendorStatusCommand request, CancellationToken cancellationToken)
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

                // Check if the status is valid
                if (!Enum.IsDefined(typeof(VendorStatus), request.Status))
                {
                    return ApiResponse<object?>.Fail(null, "Invalid status", HttpStatusCode.BadRequest);
                }

                // Check if the status is already set
                if (vendor.Status == request.Status)
                {
                    return ApiResponse<object?>.Fail(null, "Status is already set to the requested value", HttpStatusCode.BadRequest);
                }

                // Check if the vendor is already approved
                if (vendor.Status == VendorStatus.Approved && request.Status == VendorStatus.PendingApproval)
                {
                    return ApiResponse<object?>.Fail(null, "Cannot set status to Pending Approval when already Approved", HttpStatusCode.BadRequest);
                }

                // Check if the vendor is already under review
                if (vendor.Status == VendorStatus.UnderReview && request.Status == VendorStatus.Suspended)
                {
                    return ApiResponse<object?>.Fail(null, "Cannot set status to Suspended when already Under Review", HttpStatusCode.BadRequest);
                }

                // Update vendor status
                vendor.Status = request.Status;
                vendor.ModifiedAt = DateTime.UtcNow;
                await _vendorRepository.UpdateAsync(vendor, cancellationToken);

                return ApiResponse<object?>.Success(null, "Vendor status updated successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vendor");
                return ApiResponse<object?>.Fail(null, "Error updating vendor", HttpStatusCode.InternalServerError);
            }
        }
    }
}