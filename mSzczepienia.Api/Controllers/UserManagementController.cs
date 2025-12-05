using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserManagementController : ControllerBase
{
    [Authorize(Roles = "admin")]
    [HttpPost("set-role")]
    public async Task<IActionResult> SetUserRole([FromBody] SetRoleRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Role))
            return BadRequest("Invalid payload");
        
        if (request.Role != "admin" && request.Role != "worker")
            return BadRequest("Invalid role");

        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(request.UserId, new Dictionary<string, object>
        {
            { "role", request.Role }
        });

        return Ok($"Role '{request.Role}' assigned to user '{request.UserId}'");
    }
}

public class SetRoleRequest
{
    public string UserId { get; set; }
    public string Role { get; set; }
}
