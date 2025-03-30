using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Core.Interfaces.Vendors
{
    public interface IVendorRepository
    {
        Task<Vendor> GetByIdAsync(Guid id);
        Task<IEnumerable<Vendor>> GetAllAsync();
        Task<IEnumerable<Vendor>> GetByStatusAsync(VendorStatus status);
        Task AddAsync(Vendor vendor);
        Task UpdateAsync(Vendor vendor);
        Task DeleteAsync(Vendor vendor);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<Branch>> GetBranchesAsync(Guid vendorId);
        Task<IEnumerable<Product>> GetProductsAsync(Guid vendorId);
        Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync(Guid vendorId);
    }
}
