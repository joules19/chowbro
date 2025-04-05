using Chowbro.Core.Entities;
using Chowbro.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Core.Interfaces.Vendors
{
    public interface IVendorRepository
    {
        Task<Vendor> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Vendor> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Vendor>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Vendor>> GetByStatusAsync(VendorStatus status, CancellationToken cancellationToken = default);
        Task AddAsync(Vendor vendor, CancellationToken cancellationToken = default);
        Task UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default);
        Task DeleteAsync(Vendor vendor, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Branch>> GetBranchesAsync(Guid vendorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsAsync(Guid vendorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync(Guid vendorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Vendor>> GetPendingApprovalVendorsAsync(CancellationToken cancellationToken = default);
        Task<Vendor> GetVendorByIdAsync(Guid id, Func<IQueryable<Vendor>, IQueryable<Vendor>> include = null);
        Task<Vendor> GetByUserIdAsync(string userId, Func<IQueryable<Vendor>, IQueryable<Vendor>> include = null);
        Task<List<Branch>> GetBranchesByVendorIdAsync(Guid vendorId, Func<IQueryable<Branch>, IQueryable<Branch>> include = null);
        Task<Branch> GetMainBranchByVendorIdAsync(Guid vendorId, Func<IQueryable<Branch>, IQueryable<Branch>> include = null);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}