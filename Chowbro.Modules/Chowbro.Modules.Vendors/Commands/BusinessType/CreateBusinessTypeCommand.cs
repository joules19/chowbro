using Chowbro.Core.Models;
using MediatR;

namespace Chowbro.Modules.Vendors.Commands.BusinessType;

public class CreateBusinessTypeCommand : IRequest<ApiResponse<Guid>>
{
    public string Name { get; }
    public string? Description { get; }
    public bool IsActive { get; }

    public CreateBusinessTypeCommand(string name, string? description, bool isActive = true)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }
}