public interface IProductCategoryRepository
{
    Task<IEnumerable<ProductCategory>> GetByVendorAsync(Guid vendorId);
    Task<ProductCategory> GetByIdAsync(Guid id);
    Task AddAsync(ProductCategory category);
    Task UpdateAsync(ProductCategory category);
    Task DeleteAsync(ProductCategory category);
}