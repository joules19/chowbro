using Chowbro.Core.Entities.Vendor;

namespace Chowbro.Core.Repository.Interfaces.Vendor;

public interface IBusinessTypeRepository
{
    Task<BusinessType> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BusinessType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BusinessType>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(BusinessType businessType, CancellationToken cancellationToken = default);
    Task UpdateAsync(BusinessType businessType, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
