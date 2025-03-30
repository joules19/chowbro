using Chowbro.Core.Entities;

public class Branch : BaseEntity
{
    public Guid VendorId { get; set; }
    public Vendor Vendor { get; set; }

    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsMainBranch { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}