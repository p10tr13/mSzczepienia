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

        [Authorize]
        [HttpPost("new-citizen-details")]
        public async Task<IActionResult> CreateCitizen([FromBody] CitizenDetailsDto citizenDto, CancellationToken ct)
        {
            // 1. Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Get Firebase UID from token
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Invalid token: UID not found.");
            }

            // Check if the user profile already exists
            var existingUser = await _citizenService.GetCitizenDetailsAsync(firebaseUid, ct);
            if (existingUser != null)
            {
                return Conflict("User profile already exists.");
            }

            // 3. Wywo³anie serwisu
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
    }
}