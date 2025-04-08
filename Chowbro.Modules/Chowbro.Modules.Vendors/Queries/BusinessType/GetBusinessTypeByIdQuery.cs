using Chowbro.Core.Models;
using Chowbro.Modules.Vendors.DTOs.BusinessType;
using MediatR;

namespace Chowbro.Modules.Vendors.Queries.BusinessType;

public class GetBusinessTypeByIdQuery : IRequest<ApiResponse<BusinessTypeDto>>
{
    public Guid Id { get; }

    public GetBusinessTypeByIdQuery(Guid id)
    {
        Id = id;
    }
}