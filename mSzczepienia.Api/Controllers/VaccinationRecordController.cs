using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Vaccines;
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

    [Authorize]
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

    [Authorize]
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
}