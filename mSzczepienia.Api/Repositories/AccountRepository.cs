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

    public async Task CreateAccountAsync(Account account, CancellationToken cancellationToken)
    {
        _dbContext.accounts.Add(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
