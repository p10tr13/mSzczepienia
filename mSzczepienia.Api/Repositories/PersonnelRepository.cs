using mSzczepienia.Api.Repositories.Interfaces;
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
}
