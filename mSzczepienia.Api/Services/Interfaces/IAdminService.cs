using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IAdminService
{
    Task<PagedResult<AccountDto>> SearchAdminsAsync(SearchAdminQuery query, CancellationToken ct);
}
