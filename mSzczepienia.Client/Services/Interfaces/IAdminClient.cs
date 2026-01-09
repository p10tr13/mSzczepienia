using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IAdminClient
{
    Task<PagedResult<AccountDto>> SearchAdminsAsync(SearchAdminQuery query, CancellationToken ct);
    Task<DetailedAccountDto> GetAdminByIdAsync(Guid adminId, CancellationToken ct);
}
