using Chowbro.Core.Models;
using Chowbro.Modules.Vendors.DTOs.Vendor;
using MediatR;

namespace Chowbro.Modules.Vendors.Commands.Vendor
{
    public class GetVendorOnboardingStatusCommand : IRequest<ApiResponse<VendorOnboardingStatusDto>>
    {
    }
}