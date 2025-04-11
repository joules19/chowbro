using Microsoft.AspNetCore.Http;

namespace Chowbro.Modules.Vendors.DTOs.Vendor
{
    public class UpdateVendorRequest
    {
        public string? BusinessName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RcNumber { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? BusinessEmail { get; set; }

        public Guid? BusinessTypeId { get; set; }

        public string? BusinessPhoneNumber { get; set; }

        public IFormFile? LogoFile { get; set; }
        public IFormFile? CoverFile { get; set; }
    }
}