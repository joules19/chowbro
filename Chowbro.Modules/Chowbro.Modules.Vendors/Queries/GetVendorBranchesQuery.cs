using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using MediatR;

namespace Chowbro.Modules.Vendors.Queries.Vendor
{
    public class GetVendorBranchesQuery : IRequest<ApiResponse<List<BranchDto>>>
    {
        public Guid VendorId { get; }

        public GetVendorBranchesQuery(Guid vendorId)
        {
            VendorId = vendorId;
        }
    }
}