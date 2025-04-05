using Chowbro.Core.Interfaces.Vendor;
using Microsoft.EntityFrameworkCore;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Infrastructure.Persistence.Repository.Vendor
{
    public class VendorRepository : IVendorRepository
    {
        private readonly AppDbContext _context;

        public VendorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Core.Entities.Vendor.Vendor> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Vendors
                .Include(v => v.Branches)
                .Include(v => v.Products)
                .Include(v => v.ProductCategories)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        }

        public async Task<Core.Entities.Vendor.Vendor> GetVendorByIdAsync(Guid id, Func<IQueryable<Core.Entities.Vendor.Vendor>, IQueryable<Core.Entities.Vendor.Vendor>> include = null)
        {
            var query = _context.Vendors.AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Core.Entities.Vendor.Vendor> GetByUserIdAsync(string userId, Func<IQueryable<Core.Entities.Vendor.Vendor>, IQueryable<Core.Entities.Vendor.Vendor>> include = null)
        {
            var query = _context.Vendors.AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(v => v.UserId == userId);
        }

        public async Task<List<Branch>> GetBranchesByVendorIdAsync(Guid vendorId, Func<IQueryable<Branch>, IQueryable<Branch>> include = null)
        {
            var query = _context.Branches.Where(b => b.VendorId == vendorId);

            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }

        public async Task<Branch> GetMainBranchByVendorIdAsync(Guid vendorId, Func<IQueryable<Branch>, IQueryable<Branch>> include = null)
        {
            var query = _context.Branches.Where(b => b.VendorId == vendorId && b.IsMainBranch);

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Core.Entities.Vendor.Vendor>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Vendors
                .Include(v => v.Branches)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Core.Entities.Vendor.Vendor>> GetByStatusAsync(VendorStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Vendors
                .Where(v => v.Status == status)
                .Include(v => v.Branches)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Core.Entities.Vendor.Vendor vendor, CancellationToken cancellationToken = default)
        {
            await _context.Vendors.AddAsync(vendor, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Core.Entities.Vendor.Vendor vendor, CancellationToken cancellationToken = default)
        {
            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Core.Entities.Vendor.Vendor vendor, CancellationToken cancellationToken = default)
        {
            vendor.IsDeleted = true;
            vendor.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(vendor, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Vendors.AnyAsync(v => v.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Branch>> GetBranchesAsync(Guid vendorId, CancellationToken cancellationToken = default)
        {
            return await _context.Branches
                .Where(b => b.VendorId == vendorId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(Guid vendorId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.VendorId == vendorId)
                .Include(p => p.Images)
                .Include(p => p.OptionCategories)
                    .ThenInclude(oc => oc.Options)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync(Guid vendorId, CancellationToken cancellationToken = default)
        {
            return await _context.ProductCategories
                .Where(pc => pc.VendorId == vendorId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Core.Entities.Vendor.Vendor>> GetPendingApprovalVendorsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Vendors
                .Where(v => v.Status == VendorStatus.PendingApproval)
                .Include(v => v.Branches)
                .Include(v => v.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}