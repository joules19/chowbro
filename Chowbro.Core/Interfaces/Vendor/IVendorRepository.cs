using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Core.Interfaces.Vendor
{
    public interface IVendorRepository
    {
        Task<Entities.Vendor.Vendor> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Entities.Vendor.Vendor> GetByUserIdAsync(string userId, Func<IQueryable<Entities.Vendor.Vendor>, IQueryable<Entities.Vendor.Vendor>> include = null);
        Task<IEnumerable<Entities.Vendor.Vendor>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Entities.Vendor.Vendor>> GetByStatusAsync(VendorStatus status, CancellationToken cancellationToken = default);
        Task AddAsync(Entities.Vendor.Vendor vendor, CancellationToken cancellationToken = default);
        Task UpdateAsync(Entities.Vendor.Vendor vendor, CancellationToken cancellationToken = default);
        Task DeleteAsync(Entities.Vendor.Vendor vendor, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Branch>> GetBranchesAsync(Guid vendorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsAsync(Guid vendorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync(Guid vendorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Entities.Vendor.Vendor>> GetPendingApprovalVendorsAsync(CancellationToken cancellationToken = default);
        Task<Entities.Vendor.Vendor> GetVendorByIdAsync(Guid id, Func<IQueryable<Entities.Vendor.Vendor>, IQueryable<Entities.Vendor.Vendor>> include = null);
        Task<List<Branch>> GetBranchesByVendorIdAsync(Guid vendorId, Func<IQueryable<Branch>, IQueryable<Branch>> include = null);
        Task<Branch> GetMainBranchByVendorIdAsync(Guid vendorId, Func<IQueryable<Branch>, IQueryable<Branch>> include = null);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}