
namespace mSzczepienia.Api.Repositories.Interfaces;

public interface ICitizenDeviceTokenRepository
{
    Task<List<(string deviceToken, string language)>> GetTokensByCitizenIdAsync(int citizenId, CancellationToken ct);
}
