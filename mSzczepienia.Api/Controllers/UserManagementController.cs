using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Users;

namespace YourApp.Api.Controllers;

[ApiController]
[Route("api/users-management")]
public class UserManagementController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UserManagementController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [Authorize(Roles = "admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _userManagementService.CreateUserAsync(request, ct);
            return Ok(new { message = "User created successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpPost("set-citizen-role/{uid}")]
    public async Task<IActionResult> SetRole(string uid)
    {
        var claims = new Dictionary<string, object>
        {
            { "role", "citizen" }
        };
        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);
        return Ok();
    }
}