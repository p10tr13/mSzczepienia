using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO.Stats;

namespace mSzczepienia.Api.Services;

public class StatsService : IStatsService
{
    private readonly IStatsRepository _statsRepository;

    public StatsService(IStatsRepository statsRepository)
    {
        _statsRepository = statsRepository;
    }

    public async Task<DashboardStatsDto> GetAdminStatsAsync(CancellationToken ct)
    {
        return await _statsRepository.GetAdminStatsAsync(ct);
    }

    public async Task<DashboardStatsDto> GetFacilityStatsAsync(int facilityId, CancellationToken ct)
    {
        return await _statsRepository.GetFacilityStatsAsync(facilityId, ct);
    }
}
