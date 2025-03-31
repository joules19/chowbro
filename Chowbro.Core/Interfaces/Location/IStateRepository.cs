using Chowbro.Core.Entities.Location;

namespace Chowbro.Core.Interfaces.Location
{
    public interface IStateRepository
    {
        Task<State> GetByIdAsync(Guid id);
        Task<State> GetByNameAsync(string name);
        Task<IEnumerable<State>> GetAllAsync();
        Task<IEnumerable<State>> GetStatesWithLgasAsync();
        Task AddAsync(State state);
        Task UpdateAsync(State state);
    }
}