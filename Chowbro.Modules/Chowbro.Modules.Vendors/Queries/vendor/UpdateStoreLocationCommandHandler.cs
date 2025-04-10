using System.Net;
using Chowbro.Core.Entities.Vendor;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Modules.Vendors.Commands.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class UpdateStoreOperationCommandHandler
        : IRequestHandler<UpdateStoreOperationCommand, ApiResponse<bool>>

    {
        private readonly IStoreOperationRepository _storeOperationRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateStoreOperationCommandHandler> _logger;

        public UpdateStoreOperationCommandHandler(
            IStoreOperationRepository storeOperationRepo,
            ICurrentUserService currentUserService,
            ILogger<UpdateStoreOperationCommandHandler> logger)
        {
            _storeOperationRepo = storeOperationRepo;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(
            UpdateStoreOperationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var vendorId = await _currentUserService.GetVendorIdAsync();
                if (!vendorId.HasValue)
                {
                    return ApiResponse<bool>.Fail(false, "Vendor not found", HttpStatusCode.NotFound);
                }

                var operation = await _storeOperationRepo.GetByVendorIdAsync(vendorId.Value, cancellationToken)
                    ?? new StoreOperation { VendorId = vendorId.Value };

                // Update basic operation
                operation.DeliveryType = request.DeliveryType;

                if (TimeSpan.TryParse(request.OrderCutoffTime, out var cutoffTime))
                    operation.OrderCutoffTime = cutoffTime;

                if (TimeSpan.TryParse(request.MenuReadyTime, out var menuReadyTime))
                    operation.MenuReadyTime = menuReadyTime;

                // Update opening hours
                operation.OpeningHours = request.OpeningHours.Select(dto => new OpeningHours
                {
                    Day = dto.Day,
                    OpenTime = TimeSpan.Parse(dto.OpenTime),
                    CloseTime = TimeSpan.Parse(dto.CloseTime),
                    IsClosed = dto.IsClosed
                }).ToList();

                await _storeOperationRepo.UpsertAsync(operation, cancellationToken);
                return ApiResponse<bool>.Success(true, "Operation updated", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store operation");
                return ApiResponse<bool>.Fail(false, "Error updating operation", HttpStatusCode.InternalServerError);
            }
        }
    }
}