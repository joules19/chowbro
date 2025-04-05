using Chowbro.Core.Entities;
using static Chowbro.Core.Enums.Vendor;
public class Rider : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public Chowbro.Core.Enums.Rider.RiderStatus Status { get; set; } = Chowbro.Core.Enums.Rider.RiderStatus.PendingVerification;
}