
using mSzczepienia.Shared.DTO.VaccinationRecords;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface ICitizenDeviceTokenRepository
{
    Task<List<(string deviceToken, string language)>> GetTokensByCitizenIdAsync(int citizenId, CancellationToken ct);
    Task<List<UpcomingVaccinationDto>> GetDeviceTokensForNextVaccinationsAsync(CancellationToken ct);
}
