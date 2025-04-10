using System.Net;
using Chowbro.Core.Entities.Vendor;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Modules.Vendors.Queries.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class GetStoreOperationQueryHandler
        : IRequestHandler<GetStoreOperationQuery, ApiResponse<StoreOperationDto>>
    {
        private readonly IStoreOperationRepository _storeOperationRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetStoreOperationQueryHandler> _logger;

        public GetStoreOperationQueryHandler(
            IStoreOperationRepository storeOperationRepo,
            ICurrentUserService currentUserService,
            ILogger<GetStoreOperationQueryHandler> logger)
        {
            _storeOperationRepo = storeOperationRepo;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ApiResponse<StoreOperationDto>> Handle(
            GetStoreOperationQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var vendorId = await _currentUserService.GetVendorIdAsync();
                if (!vendorId.HasValue)
                {
                    return ApiResponse<StoreOperationDto>.Fail(
                        null, "Vendor not found", HttpStatusCode.NotFound);
                }

                var operation = await _storeOperationRepo.GetByVendorIdAsync(vendorId.Value, cancellationToken);
                if (operation == null)
                {
                    return ApiResponse<StoreOperationDto>.Success(
                        new StoreOperationDto(), "No operation configured", HttpStatusCode.OK);
                }

                var dto = MapToDto(operation);
                return ApiResponse<StoreOperationDto>.Success(dto, "Operation retrieved", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving store operation");
                return ApiResponse<StoreOperationDto>.Fail(
                    null, "Error retrieving operation", HttpStatusCode.InternalServerError);
            }
        }

        private StoreOperationDto MapToDto(StoreOperation operation)
        {
            return new StoreOperationDto
            {
                DeliveryType = operation.DeliveryType,
                OrderCutoffTime = operation.OrderCutoffTime?.ToString(@"hh\:mm"),
                MenuReadyTime = operation.MenuReadyTime?.ToString(@"hh\:mm"),
                OpeningHours = operation.OpeningHours.Select(oh => new OpeningHoursDto
                {
                    Day = oh.Day,
                    OpenTime = oh.OpenTime.ToString(@"hh\:mm"),
                    CloseTime = oh.CloseTime.ToString(@"hh\:mm"),
                    IsClosed = oh.IsClosed
                }).ToList()
            };
        }
    }


}