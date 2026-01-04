using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken ct);
}
