using System.Net;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Modules.Vendors.Commands.Vendor;
using Chowbro.Modules.Vendors.DTOs.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class GetVendorOnboardingStatusCommandHandler
        : IRequestHandler<GetVendorOnboardingStatusCommand, ApiResponse<VendorOnboardingStatusDto>>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetVendorOnboardingStatusCommandHandler> _logger;
        private readonly IStoreOperationRepository _storeOperationRepository;

        public GetVendorOnboardingStatusCommandHandler(
            IVendorRepository vendorRepository,
            ICurrentUserService currentUserService,
            ILogger<GetVendorOnboardingStatusCommandHandler> logger,
            IStoreOperationRepository storeOperationRepository)
        {
            _vendorRepository = vendorRepository;
            _currentUserService = currentUserService;
            _logger = logger;
            _storeOperationRepository = storeOperationRepository;
        }

        public async Task<ApiResponse<VendorOnboardingStatusDto>> Handle(
            GetVendorOnboardingStatusCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var vendorId = await _currentUserService.GetVendorIdAsync();
                if (!vendorId.HasValue)
                {
                    return ApiResponse<VendorOnboardingStatusDto>.Fail(
                        null, "Vendor not found", HttpStatusCode.NotFound);
                }

                var vendor = await _vendorRepository.GetByIdAsync(vendorId.Value, cancellationToken);
                if (vendor == null)
                {
                    return ApiResponse<VendorOnboardingStatusDto>.Fail(
                        null, "Vendor not found", HttpStatusCode.NotFound);
                }

                var status = new VendorOnboardingStatusDto
                {
                    IsPersonalInformationComplete = CheckPersonalInfoComplete(vendor),
                    IsBusinessInformationComplete = CheckBusinessInfoComplete(vendor),
                    IsLogoComplete = CheckLogoComplete(vendor),
                    IsCoverImageComplete = CheckCoverImageComplete(vendor),
                    IsBusinessAddressComplete = CheckBusinessAddressComplete(vendor),
                    IsStoreOperationComplete = CheckStoreOperationComplete(vendor)
                };

                CalculateCompletion(status);

                return ApiResponse<VendorOnboardingStatusDto>.Success(
                    status, "Onboarding status retrieved", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking onboarding status");
                return ApiResponse<VendorOnboardingStatusDto>.Fail(
                    null, "Error checking onboarding status", HttpStatusCode.InternalServerError);
            }
        }

        private bool CheckPersonalInfoComplete(Core.Entities.Vendor.Vendor vendor)
        {
            return !string.IsNullOrEmpty(vendor.FirstName) &&
                   !string.IsNullOrEmpty(vendor.LastName) &&
                   !string.IsNullOrEmpty(vendor.PhoneNumber) &&
                   !string.IsNullOrEmpty(vendor.Email);
        }

        private bool CheckBusinessInfoComplete(Core.Entities.Vendor.Vendor vendor)
        {
            return !string.IsNullOrEmpty(vendor.BusinessName) &&
                   !string.IsNullOrEmpty(vendor.Description) &&
                   !string.IsNullOrEmpty(vendor.RcNumber);
        }

        private bool CheckLogoComplete(Core.Entities.Vendor.Vendor vendor)
        {
            return !string.IsNullOrEmpty(vendor.LogoUrl);
        }

        private bool CheckCoverImageComplete(Core.Entities.Vendor.Vendor vendor)
        {
            return !string.IsNullOrEmpty(vendor.CoverImageUrl);
        }

        private bool CheckBusinessAddressComplete(Core.Entities.Vendor.Vendor vendor)
        {
            return _vendorRepository.GetMainBranchByVendorIdAsync(vendor.Id)
                .Result != null;
        }

        private bool CheckStoreOperationComplete(Core.Entities.Vendor.Vendor vendor)
        {
            return _storeOperationRepository.GetByVendorIdAsync(vendor.Id)
                 .Result != null;
        }

        private void CalculateCompletion(VendorOnboardingStatusDto status)
        {
            var completedSteps = 0;
            var totalSteps = 6; // Total number of checks

            if (status.IsPersonalInformationComplete) completedSteps++;
            if (status.IsBusinessInformationComplete) completedSteps++;
            if (status.IsLogoComplete) completedSteps++;
            if (status.IsCoverImageComplete) completedSteps++;
            if (status.IsBusinessAddressComplete) completedSteps++;
            if (status.IsStoreOperationComplete) completedSteps++;

            status.CompletionPercentage = Math.Round((decimal)completedSteps / totalSteps * 100, 2);

            // Populate pending steps
            if (!status.IsPersonalInformationComplete)
                status.PendingSteps.Add("Personal information");
            if (!status.IsBusinessInformationComplete)
                status.PendingSteps.Add("Business information");
            if (!status.IsLogoComplete)
                status.PendingSteps.Add("Logo upload");
            if (!status.IsCoverImageComplete)
                status.PendingSteps.Add("Cover image upload");
            if (!status.IsBusinessAddressComplete)
                status.PendingSteps.Add("Business address");
            if (!status.IsStoreOperationComplete)
                status.PendingSteps.Add("Store operation setup");
        }
    }
}