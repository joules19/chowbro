namespace Chowbro.Core.Services.Interfaces.Vendor;

public interface IVendorProvider
{
    Task<Entities.Vendor.Vendor?> GetVendorByUserIdAsync(string userId);
    Task<Guid?> GetVendorIdForUserAsync(string userId);
}