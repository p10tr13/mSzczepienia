using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;

namespace mSzczepienia.Api.Controllers;

[Route("api/facilities")]
[ApiController]
public class FacilityController : ControllerBase
{
    private readonly ILogger<FacilityController> _logger;
    private readonly IFacilityService _facilityService;

    public FacilityController(ILogger<FacilityController> logger, IFacilityService facilityService)
    {
        _logger = logger;
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

    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<FacilityDto?>> GetFacilityByIdAsync([FromRoute] int id, CancellationToken ct)
    {
        var result = await _facilityService.GetFacilityByIdAsync(id, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<int>> Create([FromBody] CreateFacilityDto dto, CancellationToken ct)
    {
        
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        _logger.LogInformation("Admin {FirebaseUid} is creating a facility", firebaseUid);

        try
        {
            int newId = await _facilityService.CreateFacilityAsync(dto, ct);
            return Ok(newId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a facility by admin {FirebaseUid}", firebaseUid);
            return StatusCode(500, "An error occurred while creating the facility.");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateFacilityDto dto, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        _logger.LogInformation("Admin {FirebaseUid} is updating facility {FacilityId}", firebaseUid, id);
        try
        {
            await _facilityService.UpdateFacilityAsync(id, dto, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating facility {FacilityId} by admin {FirebaseUid}", id, firebaseUid);
            return StatusCode(500, "An error occurred while updating the facility.");
        }
    }
}
