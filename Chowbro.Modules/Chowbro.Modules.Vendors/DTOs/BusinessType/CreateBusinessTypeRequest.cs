namespace Chowbro.Modules.Vendors.DTOs.BusinessType;

public class CreateBusinessTypeRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}