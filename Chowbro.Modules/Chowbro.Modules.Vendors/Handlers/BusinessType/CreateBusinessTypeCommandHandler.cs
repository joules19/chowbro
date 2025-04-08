using System.Net;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Modules.Vendors.Commands.BusinessType;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chowbro.Modules.Vendors.Handlers.BusinessType;

public class CreateBusinessTypeCommandHandler : IRequestHandler<CreateBusinessTypeCommand, ApiResponse<Guid>>
{
    private readonly IBusinessTypeRepository _repository;
    private readonly ILogger<CreateBusinessTypeCommandHandler> _logger;

    public CreateBusinessTypeCommandHandler(
        IBusinessTypeRepository repository,
        ILogger<CreateBusinessTypeCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateBusinessTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var businessType = new Core.Entities.Vendor.BusinessType
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            await _repository.AddAsync(businessType, cancellationToken);
            return ApiResponse<Guid>.Success(businessType.Id, "Business type created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating business type");
            return ApiResponse<Guid>.Fail(default, "Error creating business type", HttpStatusCode.InternalServerError);
        }
    }
}