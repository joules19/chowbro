using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using MediatR;

namespace Chowbro.Modules.Vendors.Queries
{
    public class GetVendorByUserIdQuery : IRequest<ApiResponse<VendorDto>>
    {
       // public string UserId { get; }

        public GetVendorByUserIdQuery()
        {
           // UserId = userId;
        }
    }
}