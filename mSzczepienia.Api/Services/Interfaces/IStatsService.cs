using mSzczepienia.Shared.DTO.Stats;
using System.Security.Claims;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IStatsService
{
    Task<DashboardStatsDto> GetAdminStatsAsync(CancellationToken ct);
    Task<DashboardStatsDto> GetFacilityStatsAsync(int facilityId, CancellationToken ct);
    Task<DashboardStatsDto> GetWorkerStatsAsync(ClaimsPrincipal user, CancellationToken ct);
}
