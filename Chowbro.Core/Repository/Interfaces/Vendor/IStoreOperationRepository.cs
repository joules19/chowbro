using Chowbro.Core.Entities.Vendor;

namespace Chowbro.Core.Repository.Interfaces.Vendor
{
    public interface IStoreOperationRepository
    {
        Task<StoreOperation?> GetByVendorIdAsync(Guid vendorId, CancellationToken cancellationToken = default);
        Task UpsertAsync(StoreOperation operations, CancellationToken cancellationToken);
    }
}