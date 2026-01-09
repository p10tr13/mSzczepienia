using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Controllers;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet("search")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PagedResult<AccountDto>>> SearchAdminsAsync([FromQuery] SearchAdminQuery query, CancellationToken ct)
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
            var result = await _adminService.SearchAdminsAsync(query, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for admins.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{adminId}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<DetailedAccountDto>> GetAdminByIdAsync([FromRoute] Guid adminId, CancellationToken ct)
    {
        try
        {
            var admin = await _adminService.GetAdminByIdAsync(adminId, ct);
            return Ok(admin);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving admin {AdminId}", adminId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
