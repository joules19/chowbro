using Chowbro.Core.Entities.Product;
using Chowbro.Core.Repository.Interfaces.Product;
using Microsoft.EntityFrameworkCore;

namespace Chowbro.Infrastructure.Persistence.Repository.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Core.Entities.Product.Product> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.OptionCategories)
                    .ThenInclude(oc => oc.Options)
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Branch)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Core.Entities.Product.Product>> GetByVendorAsync(Guid vendorId)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.OptionCategories)
                    .ThenInclude(oc => oc.Options)
                .Include(p => p.Category)
                .Where(p => p.VendorId == vendorId)
                .ToListAsync();
        }

        public async Task AddAsync(Core.Entities.Product.Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Core.Entities.Product.Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Core.Entities.Product.Product product)
        {
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(product);
        }


        public async Task AddOptionCategoryAsync(ProductOptionCategory optionCategory)
        {
            await _context.ProductOptionCategories.AddAsync(optionCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOptionCategoryAsync(ProductOptionCategory optionCategory)
        {
            _context.ProductOptionCategories.Update(optionCategory);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveOptionCategoryAsync(ProductOptionCategory optionCategory)
        {
            _context.ProductOptionCategories.Remove(optionCategory);
            await _context.SaveChangesAsync();
        }

        public async Task AddOptionAsync(ProductOption option)
        {
            await _context.ProductOptions.AddAsync(option);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOptionAsync(ProductOption option)
        {
            _context.ProductOptions.Update(option);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveOptionAsync(ProductOption option)
        {
            _context.ProductOptions.Remove(option);
            await _context.SaveChangesAsync();
        }

        public async Task AddImageAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task SetMainImageAsync(Guid productId, Guid imageId)
        {
            // Reset all images to not main first
            await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ForEachAsync(i => i.IsMain = false);

            // Set the specified image as main
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                image.IsMain = true;
                _context.ProductImages.Update(image);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveImageAsync(ProductImage image)
        {
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }
}