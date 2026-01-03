using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;

namespace mSzczepienia.Api.Repositories;

public class CitizenDeviceTokenRepository : ICitizenDeviceTokenRepository
{
    private readonly AppDbContext _dbContext;

    public CitizenDeviceTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<(string deviceToken, string language)>> GetTokensByCitizenIdAsync(int citizenId, CancellationToken ct)
    {
        var tokens = await _dbContext.citizenDeviceTokens
            .AsNoTracking()
            .Where(t => t.citizenId == citizenId)
            .Select(t => new { t.deviceToken, t.language })
            .ToListAsync(ct);

        return tokens.Select(t => (t.deviceToken, t.language)).ToList();
    }
}
