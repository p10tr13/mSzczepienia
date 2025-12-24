using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Controllers;

[ApiController]
[Route("api/ocrsubmission")]
public class OCRSubmissionController : ControllerBase
{
    private readonly IOCRSubmissionsService _service;
    private readonly ILogger<OCRSubmissionController> _logger;

    public OCRSubmissionController(IOCRSubmissionsService service, ILogger<OCRSubmissionController> logger)
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
}