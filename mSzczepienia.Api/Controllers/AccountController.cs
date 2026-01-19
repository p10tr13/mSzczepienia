using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Accounts;
using System.Security.Claims;

namespace mSzczepienia.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(CancellationToken ct)
        {
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(firebaseUid))
            {
                firebaseUid = User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;
            }

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Invalid token: UID not found.");
            }

            var profile = await _accountService.GetProfileAsync(firebaseUid, ct);

            if (profile == null)
            {
                return NotFound("Profile not found.");
            }

            return Ok(profile);
        }
    }
}
