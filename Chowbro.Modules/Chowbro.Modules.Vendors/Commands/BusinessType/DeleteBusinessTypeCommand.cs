using Chowbro.Core.Models;
using MediatR;

namespace Chowbro.Modules.Vendors.Commands.BusinessType;

public class DeleteBusinessTypeCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; }

    public DeleteBusinessTypeCommand(Guid id)
    {
        Id = id;
    }
}
