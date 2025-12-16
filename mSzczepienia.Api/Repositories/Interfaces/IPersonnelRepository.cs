using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IPersonnelRepository
{
    Task CreatePersonnelWithAccountAsync(Account account, Personnel personnel, CancellationToken ct);
}
