using Chowbro.Core.Models;
using MediatR;

namespace Chowbro.Modules.Vendors.Commands.BusinessType;

public class UpdateBusinessTypeCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public bool IsActive { get; }

    public UpdateBusinessTypeCommand(Guid id, string name, string? description, bool isActive)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
    }
}