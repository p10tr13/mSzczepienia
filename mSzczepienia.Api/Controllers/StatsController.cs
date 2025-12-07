using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Stats;

namespace mSzczepienia.Api.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet("admin-dashboard-stats")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<DashboardStatsDto>> GetAdminStatsAsync(CancellationToken ct)
        {
            var result = await _statsService.GetAdminStatsAsync(ct);
            return Ok(result);
        }
    }
}
