using Microsoft.EntityFrameworkCore;
using Chowbro.Core.Entities.Rider;
using Chowbro.Core.Repository.Interfaces.Rider;

namespace Chowbro.Infrastructure.Persistence.Repositories
{
    public class RiderRepository : IRiderRepository
    {
        private readonly AppDbContext _context;

        public RiderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Rider> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Riders
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Rider> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Riders
                .FirstOrDefaultAsync(r => r.UserId == userId, cancellationToken);
        }

        public async Task<IEnumerable<Rider>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Riders
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Rider rider, CancellationToken cancellationToken = default)
        {
            await _context.Riders.AddAsync(rider, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Rider rider, CancellationToken cancellationToken = default)
        {
            _context.Riders.Update(rider);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Rider rider, CancellationToken cancellationToken = default)
        {
            rider.IsDeleted = true;
            rider.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(rider, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Riders.AnyAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
