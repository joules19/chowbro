using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using Chowbro.Modules.Vendors.DTOs;
using Chowbro.Modules.Vendors.DTOs.Vendor;
using MediatR;

namespace Chowbro.Modules.Vendors.Commands.Vendor
{
    public class CompleteVendorOnboardingCommand : IRequest<ApiResponse<VendorDto>>
    {
        public CompleteVendorOnboardingRequest Model { get; }

        public CompleteVendorOnboardingCommand(CompleteVendorOnboardingRequest model)
        {
            Model = model;
        }
    }
}