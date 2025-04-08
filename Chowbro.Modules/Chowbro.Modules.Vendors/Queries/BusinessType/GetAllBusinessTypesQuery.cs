using Chowbro.Core.Models;
using Chowbro.Modules.Vendors.DTOs.BusinessType;
using MediatR;

namespace Chowbro.Modules.Vendors.Queries.BusinessType;

public class GetAllBusinessTypesQuery : IRequest<ApiResponse<IEnumerable<BusinessTypeDto>>>
{
    public bool IncludeInactive { get; }

    public GetAllBusinessTypesQuery(bool includeInactive = false)
    {
        IncludeInactive = includeInactive;
    }
}