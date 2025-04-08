using Chowbro.Core.Entities.Vendor;
using Chowbro.Core.Interfaces.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Chowbro.Core.Models;
using Chowbro.Modules.Vendors.Commands.BusinessType;

namespace Chowbro.Modules.Vendors.Handlers.BusinessType
{
    public class UpdateBusinessTypeCommandHandler : IRequestHandler<UpdateBusinessTypeCommand, ApiResponse<bool>>
    {
        private readonly IBusinessTypeRepository _repository;
        private readonly ILogger<UpdateBusinessTypeCommandHandler> _logger;

        public UpdateBusinessTypeCommandHandler(
            IBusinessTypeRepository repository,
            ILogger<UpdateBusinessTypeCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateBusinessTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var businessType = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (businessType == null)
                {
                    return ApiResponse<bool>.Fail(false, "Business type not found", HttpStatusCode.NotFound);
                }

                businessType.Name = request.Name;
                businessType.Description = request.Description;
                businessType.IsActive = request.IsActive;

                await _repository.UpdateAsync(businessType, cancellationToken);

                return ApiResponse<bool>.Success(true, "Business type updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating business type with ID: {BusinessTypeId}", request.Id);
                return ApiResponse<bool>.Fail(false, "Error updating business type", HttpStatusCode.InternalServerError);
            }
        }
    }
}