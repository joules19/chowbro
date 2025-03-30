public interface IProductRepository
{
    Task<Product> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetByVendorAsync(Guid vendorId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);

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