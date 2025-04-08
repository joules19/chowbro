namespace Chowbro.Modules.Vendors.DTOs.BusinessType;

public class BusinessTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}