using Chowbro.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Chowbro.Modules.Vendors.Commands.Vendor
{
    public class UpdateVendorCommand : IRequest<ApiResponse<object?>>
    {
        public string? BusinessName { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public string? RcNumber { get; }
        public string? Description { get; }
        public IFormFile? LogoFile { get; }
        public IFormFile? CoverImageFile { get; }
        public string? Email { get; }
        public string? PhoneNumber { get; }
        public string? BusinessEmail { get; }
        public string? BusinessPhoneNumber { get; }
        public Guid? BusinessTypeId { get; }

        public UpdateVendorCommand(
            string? businessName = null,
            string? firstName = null,
            string? lastName = null,
            string? rcNumber = null,
            string? description = null,
            IFormFile? logoFile = null,
            IFormFile? coverImageFile = null,
            string? email = null,
            string? phoneNumber = null,
            string? businessEmail = null,
            string? businessPhoneNumber = null,
            Guid? businessTypeId = null)
        {
            BusinessName = businessName;
            FirstName = firstName;
            LastName = lastName;
            RcNumber = rcNumber;
            Description = description;
            LogoFile = logoFile;
            CoverImageFile = coverImageFile;
            Email = email;
            PhoneNumber = phoneNumber;
            BusinessEmail = businessEmail;
            BusinessPhoneNumber = businessPhoneNumber;
            BusinessTypeId = businessTypeId;
        }
    }
}