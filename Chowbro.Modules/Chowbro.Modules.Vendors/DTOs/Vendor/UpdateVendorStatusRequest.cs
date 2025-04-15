using Microsoft.AspNetCore.Http;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Modules.Vendors.DTOs.Vendor
{
    public class UpdateVendorStatusRequest
    {
        public VendorStatus Status { get; set; }
    }
}