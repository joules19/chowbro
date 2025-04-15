namespace Chowbro.Core.Models.Vendor
{
    public class VendorDto
    {
        public Guid Id { get; set; }
        public string? BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RcNumber { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? CoverPublicId { get; set; }
        public string? LogoPublicId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? BusinessPhoneNumber { get; set; }
        public string? BusinessEmail { get; set; }
        public string? UserId { get; set; }
        public Guid? BusinessTypeId { get; set; }
        public string? BusinessTypeName { get; set; }

        public string? Status { get; set; }
        public List<BranchDto> Branches { get; set; } = new();
        public DateTime CreatedAt { get; set; }

    }

    public class BranchDto
    {
        public Guid? Id { get; set; }
        public Guid? Vendor { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public string? Address { get; set; }
        public string? AltAddress { get; set; }

        public string? City { get; set; }
        public Guid StateId { get; set; }
        public Guid LgaId { get; set; }

        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsMainBranch { get; set; }
    }
}
