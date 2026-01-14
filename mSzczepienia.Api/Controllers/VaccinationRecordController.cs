using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;
using System.Security.Claims;
using mSzczepienia.Shared.DTO.VaccinationRecords;

namespace mSzczepienia.Api.Controllers;

[Route("api/vaccination-records")]
[ApiController]
public class VaccinationRecordController : ControllerBase
{
    private readonly IVaccinationRecordService _vaccinationRecordService;

    public VaccinationRecordController(IVaccinationRecordService vaccinationRecordService)
    {
        _vaccinationRecordService = vaccinationRecordService;
    }

    [Authorize(Roles = "citizen")]
    [HttpGet("history")]
    public async Task<ActionResult<PagedResult<VaccinationRecordOverviewDto>>> SearchVaccinesAsync([FromQuery] GetVaccinationRecordOverviewQuery query, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        if (query.pageNumber <= 0 || query.pageSize <= 0)
        {
            return BadRequest("Page number and page size must be greater than zero.");
        }

        if (query.pageSize > 100)
        {
            return BadRequest("Page size must be less than or equal to 100.");
        }

        var result = await _vaccinationRecordService.GetVaccinationRecordsOverviewAsync(firebaseUid, query, ct);
        return Ok(result);
    }

    [Authorize(Roles = "citizen")]
    [HttpGet("details")]
    public async Task<ActionResult<List<VaccinationRecordDetailsDto>>> GetVaccinationRecordsDetailsAsync([FromQuery] GetVaccinationRecordDetailsQuery query, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        var result = await _vaccinationRecordService.GetVaccinationRecordsDetailsAsync(firebaseUid, query, ct);
        return Ok(result);
    }

    [HttpGet("by-citizen")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult<PagedResult<VaccinationRecordSimpleListItemDto>>> GetVaccinationRecordsByCitizenAsync([FromQuery] GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        if (query.PageNumber <= 0 || query.PageSize <= 0)
        {
            return BadRequest("PageNumber and PageSize must be greater than zero.");
        }

        if (query.PageSize > 100)
        {
            return BadRequest("PageSize must be less than or equal to 100.");
        }

        var result = await _vaccinationRecordService.GetVaccinationRecordsByCitizenIdAsync(firebaseUid, query, ct);
        return Ok(result);
    }

    [HttpPost("new")]
    [Authorize(Roles = "worker")]
    public async Task<IActionResult> AddVaccinationRecord([FromBody] CreateVaccinationRecordDto dto, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
            return Unauthorized("Invalid token: UID not found.");

        if (dto.citizenId <= 0 || dto.vaccineId <= 0)
            return BadRequest("CitizenId and VaccineId are required.");

        try
        {
            await _vaccinationRecordService.AddVaccinationRecordAsync(firebaseUid, dto, ct);
            return Created(string.Empty, null);
        }
        catch (InvalidOperationException invEx)
        {
            return BadRequest(invEx.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while adding vaccination record.");
        }
    }

    [HttpGet("{id}/edit")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult<VaccinationRecordEditDto>> GetVaccinationRecordForEditAsync(int id, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
            return Unauthorized("Invalid token: UID not found.");

        var result = await _vaccinationRecordService.GetVaccinationRecordByIdAsync(firebaseUid, id, ct);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "worker")]
    public async Task<IActionResult> UpdateVaccinationRecord(int id, [FromBody] UpdateVaccinationRecordDto dto, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
            return Unauthorized("Invalid token: UID not found.");

        try
        {
            await _vaccinationRecordService.UpdateVaccinationRecordAsync(firebaseUid, id, dto, ct);
            return NoContent();
        }
        catch (InvalidOperationException invEx)
        {
            return BadRequest(invEx.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating vaccination record.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "worker")]
    public async Task<IActionResult> DeleteVaccinationRecord(int id, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
            return Unauthorized("Invalid token: UID not found.");

        try
        {
            await _vaccinationRecordService.DeleteVaccinationRecordAsync(firebaseUid, id, ct);
            return NoContent();
        }
        catch (InvalidOperationException invEx)
        {
            return BadRequest(invEx.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while deleting vaccination record.");
        }
    }
}