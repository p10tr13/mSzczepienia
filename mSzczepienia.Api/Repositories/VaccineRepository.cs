using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Shared.DTO.Queries;

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

    public async Task<PagedResult<VaccineDto>> GetVaccinesAsync(GetVaccinesQuery query, CancellationToken ct)
    {
        var queryable = _dbContext.vaccineDefinitions.AsNoTracking();

        queryable = queryable.Include(v => v.vaccineDiseases).ThenInclude(vd => vd.disease);

        if (string.IsNullOrWhiteSpace(query.SearchPhrase))
        {
            if (query.Id.HasValue)
            {
                queryable = queryable.Where(v => v.vaccineId == query.Id);
            }

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                queryable = queryable.Where(v => v.vaccineName.Contains(query.Name));
            }

            if (!string.IsNullOrWhiteSpace(query.Manufacturer))
            {
                queryable = queryable.Where(v => v.manufacturer.Contains(query.Manufacturer));
            }

            if (!string.IsNullOrWhiteSpace(query.Disease))
            {
                queryable = queryable.Where(v => v.vaccineDiseases.Any(vd => vd.disease.diseaseNameEn.Contains(query.Disease) || vd.disease.diseaseNamePl.Contains(query.Disease)));
            }
        }
        else
        {
            queryable = queryable.Where(v => v.vaccineName.ToLower().Contains(query.SearchPhrase.ToLower()) ||
                v.manufacturer.ToLower().Contains(query.SearchPhrase.ToLower()) ||
                v.vaccineDiseases.Any(vd => vd.disease.diseaseNamePl.ToLower().Contains(query.SearchPhrase.ToLower()) ||
                vd.disease.diseaseNameEn.ToLower().Contains(query.SearchPhrase.ToLower()))
            );
        }

        var totalCount = await queryable.CountAsync(ct);

        queryable = queryable.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);

        var items = await queryable
            .Select(v => new VaccineDto
            {
                VaccineId = v.vaccineId,
                VaccineName = v.vaccineName,
                Manufacturer = v.manufacturer,
                DiseasesEn = v.vaccineDiseases.Select(vd => vd.disease.diseaseNameEn).ToList(),
                DiseasesPl = v.vaccineDiseases.Select(vd => vd.disease.diseaseNamePl).ToList()
            })
            .ToListAsync(ct);

        return new PagedResult<VaccineDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    public async Task<VaccineDto?> GetVaccineByIdAsync(int id, CancellationToken ct)
    {
        var vaccine = await _dbContext.vaccineDefinitions
            .AsNoTracking()
            .Include(v => v.vaccineDiseases).ThenInclude(vd => vd.disease)
            .Where(v => v.vaccineId == id)
            .Select(v => new VaccineDto
            {
                VaccineId = v.vaccineId,
                VaccineName = v.vaccineName,
                Manufacturer = v.manufacturer,
                DiseasesEn = v.vaccineDiseases.Select(vd => vd.disease.diseaseNameEn).ToList(),
                DiseasesPl = v.vaccineDiseases.Select(vd => vd.disease.diseaseNamePl).ToList()
            })
            .FirstOrDefaultAsync(ct);
        return vaccine;
    }

    public async Task<List<VaccineDto>> GetVaccinesByIdsAsync(List<int> vaccineIds, CancellationToken ct)
    {
        var vaccines = await _dbContext.vaccineDefinitions
            .AsNoTracking()
            .Include(v => v.vaccineDiseases).ThenInclude(vd => vd.disease)
            .Where(v => vaccineIds.Contains(v.vaccineId))
            .Select(v => new VaccineDto
            {
                VaccineId = v.vaccineId,
                VaccineName = v.vaccineName,
                Manufacturer = v.manufacturer,
                DiseasesEn = v.vaccineDiseases.Select(vd => vd.disease.diseaseNameEn).ToList(),
                DiseasesPl = v.vaccineDiseases.Select(vd => vd.disease.diseaseNamePl).ToList()
            })
            .ToListAsync(ct);
        return vaccines;
    }
}