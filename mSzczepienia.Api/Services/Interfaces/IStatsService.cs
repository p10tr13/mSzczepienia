using mSzczepienia.Shared.DTO.Stats;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IStatsService
{
    Task<DashboardStatsDto> GetAdminStatsAsync(CancellationToken ct);
}
