namespace Chowbro.Core.Entities.Vendor;

public class Customer : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}