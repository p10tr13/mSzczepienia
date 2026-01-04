using Microsoft.EntityFrameworkCore;
using mSzczepienia.Shared.Models;
using mSzczepienia.Api.Repositories.Interfaces;

namespace mSzczepienia.Api.Repositories;

public class DiseaseRepository : IDiseaseRepository
{
    private readonly AppDbContext _dbContext;

    public DiseaseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Disease disease, CancellationToken ct)
    {
        await _dbContext.diseases.AddAsync(disease, ct);
    }

    public async Task<Disease?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _dbContext.diseases.FirstOrDefaultAsync(d => d.diseaseId == id, ct);
    }

    public async Task<List<Disease>?> GetByNameAsNoTrackingAsync(string name, CancellationToken ct)
    {
        return await _dbContext.diseases.AsNoTracking().Where(d => (d.diseaseNameEn.Contains(name) || d.diseaseNamePl.Contains(name))).ToListAsync(ct);
    }
}
