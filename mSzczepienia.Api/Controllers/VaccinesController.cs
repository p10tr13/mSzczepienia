using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Vaccines;
using System.Security.Claims;

namespace mSzczepienia.Api.Controllers;

[Route("api/vaccines")]
[ApiController]
public class VaccinesController : ControllerBase
{
    private readonly IVaccineService _vaccineService;
    private readonly ILogger<VaccinesController> _logger;

    public VaccinesController(IVaccineService vaccineService, ILogger<VaccinesController> logger)
    {
        _vaccineService = vaccineService;
        _logger = logger;
    }

    [HttpGet("manufacturers-autocomplete")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<string>>> GetManufacturersAutocompleteAsync([FromQuery] string? query, CancellationToken ct)
    {
        var result = await _vaccineService.GetManufacturersAutocompleteAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("names-autocomplete")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<string>>> GetNamesAutocompleteAsync([FromQuery] string? query, CancellationToken ct)
    {
        var result = await _vaccineService.GetNamesAutocompleteAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("search")]
    [Authorize(Roles = "admin,worker")]
    public async Task<ActionResult<PagedResult<VaccineDto>>> SearchVaccinesAsync([FromQuery] GetVaccinesQuery query, CancellationToken ct)
    {
        if (query.PageNumber <= 0 || query.PageSize <= 0)
        {
            return BadRequest("Page number and page size must be greater than zero.");
        }

        if (query.PageSize > 100)
        {
            return BadRequest("Page size must be less than or equal to 100.");
        }

        var result = await _vaccineService.GetVaccinesAsync(query, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<VaccineDto?>> GetVaccineByIdAsync([FromRoute] int id, CancellationToken ct)
    {
        var result = await _vaccineService.GetVaccineByIdAsync(id, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("create")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateVaccineAsync([FromBody] CreateVaccineDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        _logger.LogInformation("Creating a new vaccine with name: {VaccineName} by user: {UserId}", dto.VaccineName, firebaseUid);

        try
        {
            await _vaccineService.CreateVaccineAsync(dto, ct);
            return Ok(new { message = "Vaccine created successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpGet("detailed/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<DetailedVaccineDto?>> GetDetailedVaccineByIdAsync([FromRoute] int id, CancellationToken ct)
    {
        try
        {
            var result = await _vaccineService.GetDetailedVaccineByIdAsync(id, ct);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (ArgumentException ex) {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateVaccineDto dto, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        _logger.LogInformation("Admin {FirebaseUid} is updating facility {FacilityId}", firebaseUid, id);

        try
        {
            await _vaccineService.UpdateVaccineAsync(id, dto, ct);
            return NoContent();
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating vaccine {VaccineId} by admin {FirebaseUid}", id, firebaseUid);
            return StatusCode(500, "An error occurred while updating the vaccine.");
        }
    }

    [HttpGet("vaccine-names")]
    [Authorize]
    public async Task<ActionResult<List<VaccineNameDto>>> SearchVaccineNamesAsync([FromQuery] string? query, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }
        var result = await _vaccineService.SearchVaccineNamesAsync(query, ct);
        return Ok(result);
    }
}
