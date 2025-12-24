using mSzczepienia.Shared.DTO.Stats;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IStatsClient
{
    Task<DashboardStatsDto> GetAdminStatsAsync(CancellationToken ct);
    Task<DashboardStatsDto> GetFacilityStatsAsync(int facilityId, CancellationToken ct);
    Task<DashboardStatsDto> GetWorkerStatsAsync(CancellationToken ct);
}
