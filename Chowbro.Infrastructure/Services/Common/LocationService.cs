using System.Net;
using AutoMapper;
using Chowbro.Core.Models;
using Chowbro.Core.Models.Location;
using Chowbro.Core.Repository.Interfaces.Location;
using Microsoft.Extensions.Logging;

public class LocationService : ILocationService
{
    private readonly IStateRepository _stateRepository;
    private readonly ILgaRepository _lgaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<LocationService> _logger;

    public LocationService(
        IStateRepository stateRepository,
        ILgaRepository lgaRepository,
        IMapper mapper,
        ILogger<LocationService> logger)
    {
        _stateRepository = stateRepository;
        _lgaRepository = lgaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<StateDto>>> GetAllStatesAsync()
    {
        try
        {
            var states = await _stateRepository.GetAllAsync();
            if (!states.Any())
            {
                return ApiResponse<IEnumerable<StateDto>>.Success(
                    Enumerable.Empty<StateDto>(), "No States r=record found", HttpStatusCode.OK);
            }
            var stateDtos = states.Select(state => new StateDto
            {
                Id = state.Id,
                Name = state.Name,
            }).ToList();

            return ApiResponse<IEnumerable<StateDto>>.Success(stateDtos, "States retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all states");
            return ApiResponse<IEnumerable<StateDto>>.Fail(null, "Error retrieving states", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ApiResponse<IEnumerable<LgaDto>>> GetLgasByStateIdAsync(Guid stateId)
    {
        try
        {
            var lgas = await _lgaRepository.GetByStateIdAsync(stateId);
            if (!lgas.Any())
            {
                return ApiResponse<IEnumerable<LgaDto>>.Success(
                    Enumerable.Empty<LgaDto>(), "No LGAs found for this state", HttpStatusCode.OK);
            }

            var lgaDtos = lgas.Select(state => new LgaDto
            {
                Id = state.Id,
                Name = state.Name,
            }).ToList();
            return ApiResponse<IEnumerable<LgaDto>>.Success(lgaDtos, "LGAs retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving LGAs for state {StateId}", stateId);
            return ApiResponse<IEnumerable<LgaDto>>.Fail(null, "Error retrieving LGAs", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ApiResponse<StateDto>> GetSingleStateAsync(Guid stateId)
    {
        try
        {
            var state = await _stateRepository.GetByIdAsync(stateId);
            if (state == null)
            {
                return ApiResponse<StateDto>.Fail(
                    null, "State not found", HttpStatusCode.NotFound);
            }

            var stateDto = new StateDto
            {
                Id = state.Id,
                Name = state.Name,
            };

            return ApiResponse<StateDto>.Success(stateDto, "State retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving a single state");
            return ApiResponse<StateDto>.Fail(null, "Error retrieving state", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ApiResponse<LgaDto>> GetSingleLgaAsync(Guid lgaId)
    {
        try
        {
            var lga = await _lgaRepository.GetByIdAsync(lgaId);
            if (lga == null)
            {
                return ApiResponse<LgaDto>.Fail(
                    null, "No LGA found for this state", HttpStatusCode.OK);
            }

            var lgaDto = new LgaDto
            {
                Id = lga.Id,
                Name = lga.Name,
            };

            return ApiResponse<LgaDto>.Success(lgaDto, "LGA retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving LGA");
            return ApiResponse<LgaDto>.Fail(null, "Error retrieving LGA", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ApiResponse<StateWithLgasDto>> GetStateWithLgasAsync(Guid stateId)
    {
        try
        {
            var state = await _stateRepository.GetByIdAsync(stateId);
            if (state == null)
            {
                return ApiResponse<StateWithLgasDto>.Fail(
                    null, "State not found", HttpStatusCode.NotFound);
            }

            var lgas = await _lgaRepository.GetByStateIdAsync(stateId);
            var result = new StateWithLgasDto
            {
                State = _mapper.Map<StateDto>(state),
                Lgas = _mapper.Map<IEnumerable<LgaDto>>(lgas)
            };

            return ApiResponse<StateWithLgasDto>.Success(result, "State with LGAs retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving state with LGAs for state {StateId}", stateId);
            return ApiResponse<StateWithLgasDto>.Fail(null, "Error retrieving state details", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ApiResponse<LgaDetailDto>> GetLgaDetailAsync(Guid lgaId)
    {
        try
        {
            var lga = await _lgaRepository.GetByIdAsync(lgaId);
            if (lga == null)
            {
                return ApiResponse<LgaDetailDto>.Fail(
                    null, "LGA not found", HttpStatusCode.NotFound);
            }

            var state = await _stateRepository.GetByIdAsync(lga.StateId);
            var result = new LgaDetailDto
            {
                Lga = _mapper.Map<LgaDto>(lga),
                State = _mapper.Map<StateDto>(state)
            };

            return ApiResponse<LgaDetailDto>.Success(result, "LGA details retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving LGA details for {LgaId}", lgaId);
            return ApiResponse<LgaDetailDto>.Fail(null, "Error retrieving LGA details", HttpStatusCode.InternalServerError);
        }
    }
}