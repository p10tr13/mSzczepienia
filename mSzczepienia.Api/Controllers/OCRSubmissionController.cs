using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Claims;

namespace mSzczepienia.Api.Controllers;

[ApiController]
[Route("api/ocrsubmission")]
public class OCRSubmissionController : ControllerBase
{
    private readonly IOCRSubmissionsService _service;
    private readonly ILogger<OCRSubmissionController> _logger;

    public OCRSubmissionController(IOCRSubmissionsService service, IBlobService blobService, ILogger<OCRSubmissionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("pending/worker")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult<PagedResult<SearchOCRSubmissionDTO>>> GetPendingForWorker([FromQuery] SearchOCRSubmissionQueryDto query, CancellationToken ct)
    {
        if (query.Page <= 0 || query.PageSize <= 0)
        {
            return BadRequest("Page and PageSize must be greater than zero.");
        }

        if (query.PageSize > 100)
        {
            return BadRequest("PageSize must be less than or equal to 100.");
        }

        try
        {
            var result = await _service.GetPendingForWorkerAsync(User, query, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while getting pending OCR submissions for worker.");
            return StatusCode(500);
        }
    }

    [HttpPost("process")]
    [Authorize]
    public async Task<ActionResult<List<VaccinationEntryDto>>> ProcessOcrSubmissionAsync([FromForm] List<IFormFile> files, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        if (files == null || files.Count == 0)
        {
            return BadRequest("No file has been sent");
        }

        var result = await _service.ProcessOcrSubmissionAsync(firebaseUid, files, ct);
        return Ok(result);
    }

}