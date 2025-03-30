namespace Chowbro.Core.Interfaces.Vendors
{
    public interface IVendorProvider
    {
        Task<Vendor?> GetVendorByUserIdAsync(string userId);
        Task<Guid?> GetVendorIdForUserAsync(string userId);
    }
}