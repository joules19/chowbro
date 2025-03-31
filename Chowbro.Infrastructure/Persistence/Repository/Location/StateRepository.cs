using Chowbro.Core.Entities.Location;
using Chowbro.Core.Interfaces.Location;
using Microsoft.EntityFrameworkCore;  

namespace Chowbro.Infrastructure.Persistence.Repositories.Location
{
    public class StateRepository : IStateRepository
    {
        private readonly AppDbContext _context;

        public StateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<State> GetByIdAsync(Guid id)
        {
            return await _context.States
                .Include(s => s.Lgas)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<State> GetByNameAsync(string name)
        {
            return await _context.States
                .Include(s => s.Lgas)
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _context.States
                .Include(s => s.Lgas)
                .ToListAsync();
        }

        public async Task<IEnumerable<State>> GetStatesWithLgasAsync()
        {
            return await _context.States
                .Include(s => s.Lgas)
                .Where(s => s.Lgas.Any())
                .ToListAsync();
        }

        public async Task AddAsync(State state)
        {
            await _context.States.AddAsync(state);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(State state)
        {
            _context.States.Update(state);
            await _context.SaveChangesAsync();
        }
    }
}