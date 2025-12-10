using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
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

        var cities = facilities.Select(x => x.city).ToList();

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

    public async Task<List<FacilityListDto>> GetFacilityNamesAsync(CancellationToken ct)
    {
        return await _dbContext.medicalFacilities
            .OrderBy(f => f.facilityName)
            .Select(f => new FacilityListDto
            {
                facilityId = f.facilityId,
                facilityName = f.facilityName,
            })
            .ToListAsync(ct);
    }

    public async Task<PagedResult<MedicalFacility>> SearchFacilitiesAsync(GetFacilitiesQuery query, CancellationToken ct)
    {
        var queryable = _dbContext.medicalFacilities.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.City))
        {
            queryable = queryable.Where(f => f.city.Contains(query.City));
        }

        if (!string.IsNullOrWhiteSpace(query.Street))
        {
            queryable = queryable.Where(f => f.street.Contains(query.Street));
        }

        if (!string.IsNullOrWhiteSpace(query.ZipCode))
        {
            queryable = queryable.Where(f => f.postalCode.Contains(query.ZipCode));
        }

        if (query.Id.HasValue)
        {
            queryable = queryable.Where(f => f.facilityId == query.Id.Value);
        }

        var totalItems = await queryable.CountAsync();

        var items = await queryable
            .OrderBy(f => f.facilityId)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedResult<MedicalFacility> { Items = items, TotalCount = totalItems, PageNumber = query.PageNumber, PageSize = query.PageSize };
    }
}
