using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;

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
}
