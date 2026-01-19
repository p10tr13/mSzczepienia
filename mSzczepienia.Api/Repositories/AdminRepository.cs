using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.Models;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _dbContext;

    public AdminRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<Account>> SearchAdminsAsync(SearchAdminQuery query, CancellationToken ct)
    {
        var adminsQuery = _dbContext.accounts.AsNoTracking().AsQueryable();

        adminsQuery = adminsQuery.Where(a => a.role == Role.Admin);

        if (!string.IsNullOrWhiteSpace(query.FirstName))
        {
            adminsQuery= adminsQuery.Where(a => a.name.Contains(query.FirstName));
        }

        if (!string.IsNullOrWhiteSpace(query.Surname))
        {
            adminsQuery = adminsQuery.Where(a => a.surname.Contains(query.Surname));
        }

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            adminsQuery = adminsQuery.Where(a => a.email.Contains(query.Email));
        }

        var totalCount = await adminsQuery.CountAsync(ct);

        var admins = await adminsQuery
            .OrderBy(a => a.accountId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Account>
        {
            Items = admins,
            TotalCount = totalCount,
            PageSize = query.PageSize,
            PageNumber = query.Page
        };
    }
}
