using Chowbro.Core.Interfaces.Customer;
using Microsoft.EntityFrameworkCore;

namespace Chowbro.Infrastructure.Persistence.Repository.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Core.Entities.Customer.Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Core.Entities.Customer.Customer> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        }

        public async Task<IEnumerable<Core.Entities.Customer.Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Core.Entities.Customer.Customer customer, CancellationToken cancellationToken = default)
        {
            await _context.Customers.AddAsync(customer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Core.Entities.Customer.Customer customer, CancellationToken cancellationToken = default)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Core.Entities.Customer.Customer customer, CancellationToken cancellationToken = default)
        {
            customer.IsDeleted = true;
            customer.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(customer, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
