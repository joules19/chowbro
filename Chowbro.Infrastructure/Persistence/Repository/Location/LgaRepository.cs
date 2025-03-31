using Chowbro.Core.Entities.Location;
using Chowbro.Core.Interfaces.Location;
using Microsoft.EntityFrameworkCore;  // This provides Include() and ToListAsync()

namespace Chowbro.Infrastructure.Persistence.Repositories.Location
{
    public class LgaRepository : ILgaRepository
    {
        private readonly AppDbContext _context;

        public LgaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Lga> GetByIdAsync(Guid id)
        {
            return await _context.Lgas
                .Include(l => l.State)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Lga>> GetByStateIdAsync(Guid stateId)
        {
            return await _context.Lgas
                .Where(l => l.StateId == stateId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lga>> GetByStateNameAsync(string stateName)
        {
            return await _context.Lgas
                .Include(l => l.State)
                .Where(l => l.State.Name.ToLower() == stateName.ToLower())
                .ToListAsync();
        }

        public async Task AddAsync(Lga lga)
        {
            await _context.Lgas.AddAsync(lga);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Lga> lgas)
        {
            await _context.Lgas.AddRangeAsync(lgas);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Lga lga)
        {
            _context.Lgas.Update(lga);
            await _context.SaveChangesAsync();
        }
    }
}