using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("Common")]
[Route("api/common/location")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(
        ILocationService locationService,
        ILogger<LocationsController> logger)
    {
        _locationService = locationService;
        _logger = logger;
    }

    [HttpGet("states")]
    public async Task<IActionResult> GetAllStates()
    {
        var response = await _locationService.GetAllStatesAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("states/{stateId}/lgas")]
    public async Task<IActionResult> GetLgasByStateId(Guid stateId)
    {
        var response = await _locationService.GetLgasByStateIdAsync(stateId);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("states/{stateId}")]
    public async Task<IActionResult> GetSingleState(Guid stateId)
    {
        var response = await _locationService.GetSingleStateAsync(stateId);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("lgas/{lgaId}")]
    public async Task<IActionResult> GetSingleLga(Guid lgaId)
    {
        var response = await _locationService.GetSingleLgaAsync(lgaId);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("states/{stateId}/with-lgas")]
    public async Task<IActionResult> GetStateWithLgas(Guid stateId)
    {
        var response = await _locationService.GetStateWithLgasAsync(stateId);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("lgas/{lgaId}/details")]
    public async Task<IActionResult> GetLgaDetail(Guid lgaId)
    {
        var response = await _locationService.GetLgaDetailAsync(lgaId);
        return StatusCode((int)response.StatusCode, response);
    }
}