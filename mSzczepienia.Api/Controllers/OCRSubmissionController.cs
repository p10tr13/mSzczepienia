using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
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
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

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
            // Log when worker requests submissions not limited to their facility
            if (query.SearchInWorkersFacilityOnly == false)
            {
                _logger.LogInformation("Worker (uid: {Uid}) requested OCR submissions not limited to their facility with query params: name {Name}, surname {Surname}, pesel {Pesel}.",
                    firebaseUid ?? "unknown", query.FirstName, query.Surname, query.Pesel);
            }
            var result = await _service.GetPendingForWorkerAsync(User, query, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while getting pending OCR submissions for worker.");
            return StatusCode(500);
        }
    }

    [HttpGet("review/{submissionId}/photo")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult<OCRSubmissionPhotoReviewDto>> GetSubmissionReview([FromRoute] int submissionId, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        _logger.LogInformation("OCR submission review requested. SubmissionId: {SubmissionId}, RequestedBy: {Uid}", submissionId, firebaseUid);

        try
        {
            var result = await _service.GetSubmissionPhotoReviewAsync(submissionId, ct);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving OCR submission review {SubmissionId}", submissionId);
            return StatusCode(500);
        }
    }

    [HttpGet("review/{submissionId}/entries")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult<OCRSubmissionEntriesReviewDto>> GetSubmissionEntriesReview([FromRoute] int submissionId, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        _logger.LogInformation("OCR submission entries review requested. SubmissionId: {SubmissionId}, RequestedBy: {Uid}", submissionId, firebaseUid);

        try
        {
            var result = await _service.GetSubmissionEntriesReviewAsync(submissionId, ct);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving OCR submission entries review {SubmissionId}", submissionId);
            return StatusCode(500);
        }
    }

    [HttpPost("reject/{submissionId}")]
    [Authorize(Roles = "worker")]
    public async Task<IActionResult> RejectSubmission([FromRoute] int submissionId, CancellationToken ct)
    {
        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        _logger.LogInformation("OCR submission reject requested. SubmissionId: {SubmissionId}, RequestedBy: {Uid}", submissionId, firebaseUid);

        try
        {
            var result = await _service.RejectSubmissionAsync(submissionId, ct);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException inv)
        {
            _logger.LogInformation(inv, "Invalid operation while rejecting submission {SubmissionId}", submissionId);
            return BadRequest(inv.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while rejecting OCR submission {SubmissionId}", submissionId);
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

    [HttpPost("{id}/review")]
    [Authorize(Roles = "worker")]
    public async Task<IActionResult> ProcessReview(int id, [FromBody] List<DetailedVaccinationEntryDto> entries, CancellationToken ct)
    {
        var request = new ProcessSubmissionReviewDto
        {
            SubmissionId = id,
            Entries = entries
        };

        var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return Unauthorized("Invalid token: UID not found.");
        }

        _logger.LogInformation("OCR submission review sent. SubmissionId: {SubmissionId}, RequestedBy: {Uid}", id, firebaseUid);

        try
        {
            await _service.ProcessSubmissionReviewAsync(firebaseUid, request, ct);
        }
        catch (InvalidOperationException inv)
        {
            _logger.LogInformation(inv, "Invalid operation while processing OCR submission review {SubmissionId}", id);
            return BadRequest(inv.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing OCR submission review {SubmissionId}", id);
            return StatusCode(500);
        }

        return Ok();
    }
}