using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories;

public class VaccineRepository : IVaccineRepository
{
    private readonly AppDbContext _dbContext;

    public VaccineRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> GetNamesAutocompleteAsync(string? query, CancellationToken ct)
    {
        var q = _dbContext.vaccineDefinitions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query))
        {
            q = q.Where(v => v.vaccineName.Contains(query));
        }

        return await q
            .Select(v => v.vaccineName)
            .Distinct()
            .OrderBy(v => v)
            .Take(10)
            .ToListAsync(ct);
    }

    public async Task<List<string>> GetManufacturersAutocompleteAsync(string? query, CancellationToken ct)
    {
        var q = _dbContext.vaccineDefinitions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query))
        {
            q = q.Where(v => v.manufacturer.Contains(query));
        }

        return await q
            .Select(v => v.manufacturer)
            .Distinct()
            .OrderBy(v => v)
            .Take(10)
            .ToListAsync(ct);
    }
}
