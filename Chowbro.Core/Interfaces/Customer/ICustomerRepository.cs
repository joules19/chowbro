namespace Chowbro.Core.Interfaces.Customer
{
    public interface ICustomerRepository
    {
        Task<Entities.Customer.Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Entities.Customer.Customer> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Entities.Customer.Customer>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Entities.Customer.Customer customer, CancellationToken cancellationToken = default);
        Task UpdateAsync(Entities.Customer.Customer customer, CancellationToken cancellationToken = default);
        Task DeleteAsync(Entities.Customer.Customer customer, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}