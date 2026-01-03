using mSzczepienia.Shared.Models;
using mSzczepienia.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace mSzczepienia.Api.Repositories;

public class VaccinationEntryRepository : IVaccinationEntryRepository
{
    private readonly AppDbContext _dbContext;

    public VaccinationEntryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<VaccinationEntry>> GetBySubmissionIdAsync(int ocrSubmissionId, CancellationToken ct)
    {
        return await _dbContext.vaccinationEntries
            .Where(ve => ve.ocrSubmissionId == ocrSubmissionId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(VaccinationEntry entry, CancellationToken ct)
    {
        await _dbContext.vaccinationEntries.AddAsync(entry, ct);
    }

    public void Update(VaccinationEntry entry)
    {
        _dbContext.vaccinationEntries.Update(entry);
    }
}
