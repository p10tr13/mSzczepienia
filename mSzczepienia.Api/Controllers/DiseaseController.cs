using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Diseases;

namespace mSzczepienia.Api.Controllers;


[Route("api/diseases")]
[ApiController]
public class DiseaseController : ControllerBase
{
    private readonly IDiseaseService _diseaseService;

    public DiseaseController(IDiseaseService diseaseService)
    {
        _diseaseService = diseaseService;
    }

    [HttpGet("names-autocomplete")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<DiseaseDto>>> GetDiseasesAutocompleteAsync([FromQuery] string query, [FromQuery] int count, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query) || count <= 0)
        {
            return BadRequest("Invalid query or count parameter.");
        }

        var result = await _diseaseService.GetDiseasesAutocompleteAsync(query, count, ct);
        return Ok(result);
    }
}
