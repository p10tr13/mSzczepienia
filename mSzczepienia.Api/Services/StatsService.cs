using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Stats;
using System.Security.Claims;

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

    public async Task<DashboardStatsDto> GetWorkerStatsAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        string? firebaseUid = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            firebaseUid = user.FindFirst("user_id")?.Value ?? user.FindFirst("uid")?.Value;
        }

        if (string.IsNullOrEmpty(firebaseUid))
        {
            throw new UnauthorizedAccessException("User identifier (Firebase UID) is missing in the token.");
        }

        return await _statsRepository.GetWorkerStatsByFirebaseUidAsync(firebaseUid, ct);
    }
}
