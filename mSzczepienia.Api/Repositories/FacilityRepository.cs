using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories;

public class FacilityRepository : IFacilityRepository
{
    private readonly AppDbContext _dbContext;

    public FacilityRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct)
    {
        var q = _dbContext.medicalFacilities.AsNoTracking();

        List<MedicalFacility> facilities = await q.ToListAsync(ct);

        var cities = facilities.Select(x => x.address.Split(',')[1].Trim()).ToList();

        if (string.IsNullOrWhiteSpace(query))
        {
            return cities
                .Distinct()
                .OrderBy(x => x)
                .Take(10)
                .ToList();
        }

        return cities
            .Where(x => x.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .OrderBy(x => x)
            .Take(10)
            .ToList();

        // To do: tak powinno być
        //if (!string.IsNullOrWhiteSpace(query))
        //{
        //    query = query.Where(x => x.City.StartsWith(query));
        //}
        //return await q
        //    .Select(x => x.City)
        //    .Distinct()
        //    .OrderBy(x => x)
        //    .Take(10)
        //    .ToListAsync(ct);
    }
}
