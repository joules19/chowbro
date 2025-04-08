using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Modules.Vendors.Commands.BusinessType;

namespace Chowbro.Modules.Vendors.Handlers.BusinessType
{
    public class DeleteBusinessTypeCommandHandler : IRequestHandler<DeleteBusinessTypeCommand, ApiResponse<bool>>
    {
        private readonly IBusinessTypeRepository _repository;
        private readonly ILogger<DeleteBusinessTypeCommandHandler> _logger;

        public DeleteBusinessTypeCommandHandler(
            IBusinessTypeRepository repository,
            ILogger<DeleteBusinessTypeCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteBusinessTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var businessType = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (businessType == null)
                {
                    return ApiResponse<bool>.Fail(false, "Business type not found", HttpStatusCode.NotFound);
                }

                await _repository.DeleteAsync(request.Id, cancellationToken);

                return ApiResponse<bool>.Success(true, "Business type deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting business type with ID: {BusinessTypeId}", request.Id);
                return ApiResponse<bool>.Fail(false, "Error deleting business type", HttpStatusCode.InternalServerError);
            }
        }
    }
}