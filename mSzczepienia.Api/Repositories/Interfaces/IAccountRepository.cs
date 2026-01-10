using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken ct);
    Task<Account?> GetByIdAsync(Guid accountId, CancellationToken ct);
    Task<Account?> GetByEmailAsync(string email, CancellationToken ct);
    void Update(Account account);
}
