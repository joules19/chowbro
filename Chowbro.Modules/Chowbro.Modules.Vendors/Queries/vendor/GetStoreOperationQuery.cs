using Chowbro.Core.Models;
using MediatR;

namespace Chowbro.Modules.Vendors.Queries.Vendor
{
    public class GetStoreOperationQuery : IRequest<ApiResponse<StoreOperationDto>>
    {
    }
}