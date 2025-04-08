using Chowbro.Core.Repository.Interfaces.Product;
using Microsoft.EntityFrameworkCore;

namespace Chowbro.Infrastructure.Persistence.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly AppDbContext _context;

        public ProductCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductCategory>> GetByVendorAsync(Guid vendorId)
        {
            return await _context.ProductCategories
                .Include(pc => pc.Products)
                .Where(pc => pc.VendorId == vendorId)
                .ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(Guid id)
        {
            return await _context.ProductCategories
                .Include(pc => pc.Products)
                .FirstOrDefaultAsync(pc => pc.Id == id);
        }

        public async Task AddAsync(ProductCategory category)
        {
            await _context.ProductCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductCategory category)
        {
            _context.ProductCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductCategory category)
        {
            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}