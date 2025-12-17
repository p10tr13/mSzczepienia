using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;

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

    [HttpGet("facility-names")]
    public async Task<ActionResult<List<FacilityListDto>>> GetFacilityNamesAutocompleteAsync([FromQuery] string? query, CancellationToken ct)
    {
        var result = await _facilityService.GetFacilityNamesAutocompleteAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("search")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PagedResult<SearchFacilityDto>>> SearchFacilitiesAsync([FromQuery] GetFacilitiesQuery query, CancellationToken ct)
    {
        if (query.PageNumber <= 0 || query.PageSize <= 0)
        {
            return BadRequest("PageNumber and PageSize must be greater than zero.");
        }

        if (query.PageSize > 100)
        {
            return BadRequest("PageSize must be less than or equal to 100.");
        }

        var result = await _facilityService.SearchFacilitiesAsync(query, ct);
        return Ok(result);
    }
}
