using Chowbro.Core.Models;
using Chowbro.Core.Models.Location;

public interface ILocationService
{
    Task<ApiResponse<IEnumerable<StateDto>>> GetAllStatesAsync();
    Task<ApiResponse<IEnumerable<LgaDto>>> GetLgasByStateIdAsync(Guid stateId);
    Task<ApiResponse<StateDto>> GetSingleStateAsync(Guid stateId);
    Task<ApiResponse<LgaDto>> GetSingleLgaAsync(Guid lgaId);
    Task<ApiResponse<StateWithLgasDto>> GetStateWithLgasAsync(Guid stateId);
    Task<ApiResponse<LgaDetailDto>> GetLgaDetailAsync(Guid lgaId);
}