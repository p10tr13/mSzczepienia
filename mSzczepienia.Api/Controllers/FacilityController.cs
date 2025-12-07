using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;

namespace mSzczepienia.Api.Controllers;

[Route("api/facilities")]
[ApiController]
public class FacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public FacilityController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [HttpGet("cities-autocomplete")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<string>>> GetCitiesAutocompleteAsync([FromQuery] string? query, CancellationToken ct)
    {
        var result = await _facilityService.GetCitiesAutocompleteAsync(query, ct);
        return Ok(result);
    }
}
