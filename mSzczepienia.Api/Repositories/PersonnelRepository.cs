using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories;

public class PersonnelRepository : IPersonnelRepository
{
    private readonly AppDbContext _dbContext;

    public PersonnelRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Personnel personnel, CancellationToken ct)
    {
        await _dbContext.personnel.AddAsync(personnel, ct);
    }

    public async Task<PagedResult<PersonnelDto>> GetFacilityPersonnelAsync(GetPersonnelQuery query, CancellationToken ct)
    {   
        var queryable = _dbContext.personnel.AsNoTracking().Include(p => p.account).AsQueryable();
        queryable = queryable.Where(p => p.facilityId == query.FacilityId);

        var totalCount = await queryable.CountAsync(ct);
        var items = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PersonnelDto
            {
                Id = p.personnelId,
                AccountId = p.accountId,
                Name = p.account.name,
                Surname = p.account.surname,
                FacilityId = p.facilityId,
                PwzNumber = p.pwzNumber,
                Position = p.position.ToString(),
                Email = p.account.email,
                DateOfBirth = p.account.dateOfBirth
            })
            .ToListAsync(ct);
        return new PagedResult<PersonnelDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    public async Task<int?> GetFacilityIdForWorkerByFirebaseUidAsync(string firebaseUid, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
        {
            return null;
        }

        var personnel = await _dbContext.personnel.AsNoTracking().Include(p => p.account).FirstOrDefaultAsync(p => p.account.firebaseUid == firebaseUid, ct);

        return personnel?.facilityId;
    }

    public async Task<Personnel?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken ct)
    {
        return await _dbContext.personnel
            .Include(p => p.account)
            .FirstOrDefaultAsync(p => p.account.firebaseUid == firebaseUid, ct);
    }

    public async Task<PagedResult<Personnel>> SearchPersonnelAsync(SearchPersonnelQuery query, CancellationToken ct)
    {
        var queryable = _dbContext.personnel.AsNoTracking().Include(p => p.account).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.FirstName))
        {
            queryable = queryable.Where(p => p.account.name.Contains(query.FirstName));
        }

        if (!string.IsNullOrWhiteSpace(query.Surname))
        {
            queryable = queryable.Where(p => p.account.surname.Contains(query.Surname));
        }

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            queryable = queryable.Where(p => p.account.email.Contains(query.Email));
        }

        if (!string.IsNullOrWhiteSpace(query.FacilityCity))
        {
            queryable = queryable.Where(p => p.medicalFacility.city.Contains(query.FacilityCity));
        }

        if (!string.IsNullOrWhiteSpace(query.FacilityName))
        {
            queryable = queryable.Where(p => p.medicalFacility.facilityName.Contains(query.FacilityName));
        }

        if (!string.IsNullOrWhiteSpace(query.FacilityPostalCode))
        {
            queryable = queryable.Where(p => p.medicalFacility.postalCode.Contains(query.FacilityPostalCode));
        }

        if (!string.IsNullOrWhiteSpace(query.FacilityStreet))
        {
            queryable = queryable.Where(p => p.medicalFacility.street.Contains(query.FacilityStreet));
        }

        var totalCount = await queryable.CountAsync(ct);

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Personnel>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.Page,
            PageSize = query.PageSize
        };
    }

    public void Update(Personnel personnel)
    {
        _dbContext.personnel.Update(personnel);
    }

    public async Task<Personnel?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _dbContext.personnel
            .Include(p => p.account)
            .FirstOrDefaultAsync(p => p.personnelId == id, ct);
    }
}
