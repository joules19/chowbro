using Chowbro.Core.Entities.Product;

namespace Chowbro.Core.Repository.Interfaces.Product
{
    public interface IProductRepository
    {
        Task<Entities.Product.Product> GetByIdAsync(Guid id);
        Task<IEnumerable<Entities.Product.Product>> GetByVendorAsync(Guid vendorId);
        Task AddAsync(Entities.Product.Product product);
        Task UpdateAsync(Entities.Product.Product product);
        Task DeleteAsync(Entities.Product.Product product);

        // Option categories
        Task AddOptionCategoryAsync(ProductOptionCategory optionCategory);
        Task UpdateOptionCategoryAsync(ProductOptionCategory optionCategory);
        Task RemoveOptionCategoryAsync(ProductOptionCategory optionCategory);

        // Options
        Task AddOptionAsync(ProductOption option);
        Task UpdateOptionAsync(ProductOption option);
        Task RemoveOptionAsync(ProductOption option);

        // Images
        Task AddImageAsync(ProductImage image);
        Task SetMainImageAsync(Guid productId, Guid imageId);
        Task RemoveImageAsync(ProductImage image);
    }
}