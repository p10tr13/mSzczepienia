using mSzczepienia.Shared.Models;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<PagedResult<Account>> SearchAdminsAsync(SearchAdminQuery query, CancellationToken ct);
}
