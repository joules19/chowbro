using Microsoft.AspNetCore.Http;

namespace Chowbro.Modules.Vendors.DTOs.Vendor
{
    public class UpdateVendorRequest
    {
        public string? BusinessName { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public string? RcNumber { get; }
        public string? Description { get; }
        public string? Email { get; }
        public string? PhoneNumber { get; }
        
        public IFormFile? LogoFile { get; }
        public IFormFile? CoverFile { get; }
    }
}