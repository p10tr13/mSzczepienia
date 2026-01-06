using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<PagedResult<AccountDto>> SearchAdminsAsync(SearchAdminQuery query, CancellationToken ct)
    {
        var accounts = await _adminRepository.SearchAdminsAsync(query, ct);

        var items = accounts.Items.Select(a => new AccountDto
        {
            Id = a.accountId,
            Name = a.name,
            Surname = a.surname,
            Email = a.email
        }).ToList();

        return new PagedResult<AccountDto>
        {
            Items = items,
            TotalCount = accounts.TotalCount,
            PageNumber = accounts.PageNumber,
            PageSize = accounts.PageSize
        };
    }
}
