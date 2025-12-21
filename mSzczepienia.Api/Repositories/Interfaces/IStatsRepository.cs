using mSzczepienia.Shared.DTO.Stats;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IStatsRepository
{
    Task<DashboardStatsDto> GetAdminStatsAsync(CancellationToken ct);
    Task<DashboardStatsDto> GetFacilityStatsAsync(int facilityId, CancellationToken ct);
}
