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

    public async Task CreatePersonnelWithAccountAsync(Account account, Personnel personnel, CancellationToken ct)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        try
        {
            _dbContext.accounts.Add(account);
            await _dbContext.SaveChangesAsync(ct);

            personnel.accountId = account.accountId;
            _dbContext.personnel.Add(personnel);
            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
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
}
