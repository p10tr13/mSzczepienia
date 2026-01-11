using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Shared.Models;

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

    public async Task<List<UpcomingVaccinationDto>> GetDeviceTokensForNextVaccinationsAsync(CancellationToken ct)
    {
        var tokens = new List<UpcomingVaccinationDto>();

        var targetDate = DateTime.Today.AddDays(7);

        var vaccinationRecords = await _dbContext.vaccinationRecords
            .AsNoTracking()
            .Where(c => c.nextVaccinationDate == targetDate)
            .ToListAsync();

        foreach (var record in vaccinationRecords)
        {
            var token = await _dbContext.citizenDeviceTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.citizenId == record.citizenId, ct);


            if (token != null)
            {
                tokens.Add(new UpcomingVaccinationDto
                {
                    vaccineName = record.vaccineDefinition.vaccineName,
                    deviceToken = token.deviceToken,
                    language = token.language,
                });
            }
        }

        return tokens;
    }
}
