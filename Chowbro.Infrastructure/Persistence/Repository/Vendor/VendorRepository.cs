using Chowbro.Core.Entities;
using Chowbro.Core.Interfaces.Vendors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Infrastructure.Persistence.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly AppDbContext _context;

        public VendorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Vendor> GetByIdAsync(Guid id)
        {
            return await _context.Vendors
                .Include(v => v.Branches)
                .Include(v => v.Products)
                .Include(v => v.ProductCategories)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            return await _context.Vendors
                .Include(v => v.Branches)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vendor>> GetByStatusAsync(VendorStatus status)
        {
            return await _context.Vendors
                .Where(v => v.Status == status)
                .Include(v => v.Branches)
                .ToListAsync();
        }

        public async Task AddAsync(Vendor vendor)
        {
            await _context.Vendors.AddAsync(vendor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vendor vendor)
        {
            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Vendor vendor)
        {
            vendor.IsDeleted = true;
            vendor.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(vendor);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Vendors.AnyAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Branch>> GetBranchesAsync(Guid vendorId)
        {
            return await _context.Branches
                .Where(b => b.VendorId == vendorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(Guid vendorId)
        {
            return await _context.Products
                .Where(p => p.VendorId == vendorId)
                .Include(p => p.Images)
                .Include(p => p.OptionCategories)
                    .ThenInclude(oc => oc.Options)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync(Guid vendorId)
        {
            return await _context.ProductCategories
                .Where(pc => pc.VendorId == vendorId)
                .ToListAsync();
        }
    }
}