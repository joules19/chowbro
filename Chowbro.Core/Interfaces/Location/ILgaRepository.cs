using Chowbro.Core.Entities.Location;

namespace Chowbro.Core.Interfaces.Location
{
    public interface ILgaRepository
    {
        Task<Lga> GetByIdAsync(Guid id);
        Task<IEnumerable<Lga>> GetByStateIdAsync(Guid stateId);
        Task<IEnumerable<Lga>> GetByStateNameAsync(string stateName);
        Task AddAsync(Lga lga);
        Task AddRangeAsync(IEnumerable<Lga> lgas);
        Task UpdateAsync(Lga lga);
    }
}