using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _dbContext;

    public AccountRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Account account, CancellationToken ct)
    {
        await _dbContext.accounts.AddAsync(account, ct);
    }

    public async Task<Account?> GetByIdAsync(Guid accountId, CancellationToken ct)
    {
        return await _dbContext.accounts
            .FirstOrDefaultAsync(a => a.accountId == accountId, ct);
    }

    public void Update(Account account)
    {
        _dbContext.accounts.Update(account);
    }
}