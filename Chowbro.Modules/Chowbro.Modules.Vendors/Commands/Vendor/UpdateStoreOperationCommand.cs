using Chowbro.Core.Entities.Vendor;
using Chowbro.Core.Models;
using MediatR;

namespace Chowbro.Modules.Vendors.Commands.Vendor
{
    public class UpdateStoreOperationCommand : IRequest<ApiResponse<bool>>
    {
        public DeliveryType DeliveryType { get; set; }
        public string? OrderCutoffTime { get; set; }
        public string? MenuReadyTime { get; set; }
        public List<OpeningHoursDto> OpeningHours { get; set; } = new();
    }
}