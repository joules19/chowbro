namespace Chowbro.Core.Interfaces.Rider
{
    public interface IRiderRepository
    {
        Task<global::Chowbro.Core.Entities.Rider.Rider> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<global::Chowbro.Core.Entities.Rider.Rider> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<global::Chowbro.Core.Entities.Rider.Rider>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(global::Chowbro.Core.Entities.Rider.Rider rider, CancellationToken cancellationToken = default);
        Task UpdateAsync(global::Chowbro.Core.Entities.Rider.Rider rider, CancellationToken cancellationToken = default);
        Task DeleteAsync(global::Chowbro.Core.Entities.Rider.Rider rider, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}