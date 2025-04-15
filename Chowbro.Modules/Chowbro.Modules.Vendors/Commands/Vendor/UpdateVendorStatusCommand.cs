using Chowbro.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Modules.Vendors.Commands.Vendor
{
    public class UpdateVendorStatusCommand : IRequest<ApiResponse<object?>>
    {
        public VendorStatus Status { get; set; }

        public UpdateVendorStatusCommand(
            VendorStatus status)
        {
            Status = status;
        }
    }
}