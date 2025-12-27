using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Vaccines;

namespace mSzczepienia.Api.Controllers;

[Route("api/vaccines")]
[ApiController]
public class VaccinesController : ControllerBase
{
    private readonly IVaccineService _vaccineService;

    public VaccinesController(IVaccineService vaccineService)
    {
        _vaccineService = vaccineService;
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
}
