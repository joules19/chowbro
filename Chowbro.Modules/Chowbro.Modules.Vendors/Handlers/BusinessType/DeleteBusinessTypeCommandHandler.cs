using Chowbro.Core.Interfaces.Vendor;
using Chowbro.Core.Models.Responses;
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
    public class DeleteBusinessTypeCommandHandler : IRequestHandler<DeleteBusinessTypeCommand, ApiResponse<>>
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

        public async Task<ApiResponse> Handle(DeleteBusinessTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var businessType = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (businessType == null)
                {
                    return ApiResponse.Fail("Business type not found", HttpStatusCode.NotFound);
                }

                await _repository.DeleteAsync(request.Id, cancellationToken);

                return ApiResponse.Success("Business type deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting business type with ID: {BusinessTypeId}", request.Id);
                return ApiResponse.Fail("Error deleting business type", HttpStatusCode.InternalServerError);
            }
        }
    }
}