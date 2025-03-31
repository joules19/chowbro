using Chowbro.Core.Models.Vendor;
using Microsoft.AspNetCore.Http;

namespace Chowbro.Modules.Vendors.DTOs.Vendor
{
    public class CompleteVendorOnboardingRequest
    {
        public string Name { get; set; }
        public string RcNumber { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public IFormFile Logo { get; set; }
        public IFormFile CoverImage { get; set; }
        public BranchDto MainBranch { get; set; }
    }
}