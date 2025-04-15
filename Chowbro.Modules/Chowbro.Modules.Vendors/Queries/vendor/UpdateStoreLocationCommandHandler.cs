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

                if (!TryParseTime(request.OrderCutoffTime, out var orderCutoffTime) ||
                    !TryParseTime(request.MenuReadyTime, out var menuReadyTime))
                {
                    return ApiResponse<bool>.Fail(false, "Invalid time format. Use HH:mm", HttpStatusCode.BadRequest);
                }

                var openingHours = new List<OpeningHours>();
                foreach (var ohDto in request.OpeningHours)
                {
                    if (!TimeSpan.TryParseExact(ohDto.OpenTime, "hh\\:mm", null, out var openTime) ||
                        !TimeSpan.TryParseExact(ohDto.CloseTime, "hh\\:mm", null, out var closeTime))
                    {
                        return ApiResponse<bool>.Fail(false, $"Invalid time format for {ohDto.Day}. Use HH:mm", HttpStatusCode.BadRequest);
                    }

                    if (!ohDto.IsClosed && openTime >= closeTime)
                    {
                        return ApiResponse<bool>.Fail(false, $"Close time must be after open time for {ohDto.Day}", HttpStatusCode.BadRequest);
                    }

                    openingHours.Add(new OpeningHours
                    {
                        Day = ohDto.Day,
                        OpenTime = openTime,
                        CloseTime = closeTime,
                        IsClosed = ohDto.IsClosed
                    });
                }

                var storeOperation = new StoreOperation
                {
                    VendorId = vendorId.Value,
                    DeliveryType = request.DeliveryType,
                    OrderCutoffTime = orderCutoffTime,
                    MenuReadyTime = menuReadyTime,
                    OpeningHours = openingHours
                };

                await _storeOperationRepo.UpsertAsync(storeOperation, cancellationToken);

                return ApiResponse<bool>.Success(true, "Store operation updated successfully", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store operations");
                return ApiResponse<bool>.Fail(false, "An error occurred while updating store operations", HttpStatusCode.InternalServerError);
            }
        }

        private bool TryParseTime(string timeString, out TimeSpan? time)
        {
            if (string.IsNullOrEmpty(timeString))
            {
                time = null;
                return true;
            }

            if (TimeSpan.TryParseExact(timeString, "hh\\:mm", null, out var parsedTime))
            {
                time = parsedTime;
                return true;
            }

            time = null;
            return false;
        }

        //     public async Task<ApiResponse<bool>> Handle(
        //   UpdateStoreOperationCommand request,
        //   CancellationToken cancellationToken)
        //     {
        //         try
        //         {
        //             var vendorId = await _currentUserService.GetVendorIdAsync();
        //             if (!vendorId.HasValue)
        //             {
        //                 return ApiResponse<bool>.Fail(false, "Vendor not found", HttpStatusCode.NotFound);
        //             }

        //             // Load existing to get the ID if it exists
        //             var existing = await _storeOperationRepo.GetByVendorIdAsync(vendorId.Value, cancellationToken);

        //             // Create operation with parsed times
        //             var operation = new StoreOperation
        //             {
        //                 // Preserve existing ID if updating
        //                 Id = existing?.Id ?? Guid.Empty, // Will be set properly in repository
        //                 VendorId = vendorId.Value,
        //                 DeliveryType = request.DeliveryType,
        //                 OrderCutoffTime = ParseTimeSpan(request.OrderCutoffTime),
        //                 MenuReadyTime = ParseTimeSpan(request.MenuReadyTime),
        //                 OpeningHours = request.OpeningHours.Select(dto => new OpeningHours
        //                 {
        //                     Day = dto.Day,
        //                     OpenTime = ParseTimeSpan(dto.OpenTime) ?? TimeSpan.Zero,
        //                     CloseTime = ParseTimeSpan(dto.CloseTime) ?? TimeSpan.Zero,
        //                     IsClosed = dto.IsClosed
        //                 }).ToList()
        //             };

        //             await _storeOperationRepo.UpsertAsync(operation, cancellationToken);
        //             return ApiResponse<bool>.Success(true, "Operation updated", HttpStatusCode.OK);
        //         }
        //         catch (Exception ex)
        //         {
        //             _logger.LogError(ex, "Error updating store operation");
        //             return ApiResponse<bool>.Fail(false, "Error updating operation", HttpStatusCode.InternalServerError);
        //         }
        //     }

        //     private TimeSpan? ParseTimeSpan(string timeString)
        //     {
        //         return TimeSpan.TryParse(timeString, out var time) ? time : null;
        //     }
    }
}