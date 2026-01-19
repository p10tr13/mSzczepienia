using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using System.Security.Claims;

namespace mSzczepienia.Api.Controllers
{
    [Route("api/citizens")]
    [ApiController]
    public class CitizenController : ControllerBase
    {
        private readonly ICitizenService _citizenService;

        public CitizenController(ICitizenService citizenService)
        {
            _citizenService = citizenService;
        }

        [HttpPost("device-token")]
        public async Task<IActionResult> RegisterDeviceToken([FromBody] GetDeviceTokenQuery deviceToken, CancellationToken ct)
        {
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Invalid token: UID not found.");
            }
            try
            {
                await _citizenService.RegisterDeviceTokenAsync(firebaseUid, deviceToken.deviceToken, deviceToken.language, ct);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while registering the device token.");
            }
            return Ok("Device token registered successfully.");
        }

        [Authorize]
        [HttpGet("citizen-details")]
        public async Task<IActionResult> GetCitizenDetails(CancellationToken ct)
        {
            // 1. Get Firebase UID
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Invalid token: UID not found.");
            }

            // 2. Service Layer
            var result = await _citizenService.GetCitizenDetailsAsync(firebaseUid, ct);

            // 3. HTTP Layer (Response handle)
            if (result == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(result);
        }

        [Authorize(Roles = "worker")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCitizenById([FromRoute] int id, CancellationToken ct)
        {
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Invalid token: UID not found.");
            }

            var result = await _citizenService.GetCitizenDetailsByIdAsync(firebaseUid, id, ct);

            if (result == null)
            {
                return NotFound("Citizen not found.");
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("new-citizen-details")]
        public async Task<IActionResult> CreateCitizen([FromBody] CitizenDetailsDto citizenDto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Invalid token: UID not found.");
            }

            var existingUser = await _citizenService.GetCitizenDetailsAsync(firebaseUid, ct);
            if (existingUser != null)
            {
                return Conflict("User profile already exists.");
            }

            try
            {
                await _citizenService.CreateCitizenAsync(firebaseUid, citizenDto, ct);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the profile.");
            }

            return CreatedAtAction(nameof(GetCitizenDetails), new { }, citizenDto);
        }

        [HttpGet("search")]
        [Authorize(Roles = "worker")]
        public async Task<ActionResult<PagedResult<SimpleCitizenDto>>> GetCitizensAsync([FromQuery] GetCitizensQuery query, CancellationToken ct)
        {
            if (query.PageNumber <= 0 || query.PageSize <= 0)
            {
                return BadRequest("PageNumber and PageSize must be greater than zero.");
            }

            if (query.PageSize > 100)
            {
                return BadRequest("PageSize must be less than or equal to 100.");
            }

            return await _citizenService.SearchCitizensAsync(User, query, ct);
        }

        [HttpGet("submission-status")]
        [Authorize(Roles = "citizen")]
        public async Task<ActionResult<CitizenSubmissionStatusDto>> GetCitizenSubmissionStatus(CancellationToken ct)
        {
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Invalid token: UID not found.");
            }

            var result = await _citizenService.GetCitizenSubmissionStatusAsync(firebaseUid, ct);

            if (result == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(result);
        }
    }
}