using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using MediatR;

namespace Chowbro.Modules.Vendors.Queries.Vendor
{
    public class GetVendorByIdQuery : IRequest<ApiResponse<VendorDto>>
    {
        public Guid VendorId { get; }

        public GetVendorByIdQuery(Guid vendorId)
        {
            VendorId = vendorId;
        }
    }
}