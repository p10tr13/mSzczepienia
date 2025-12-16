using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IAccountRepository
{
    Task CreateAccountAsync(Account account, CancellationToken cancellationToken);
}
