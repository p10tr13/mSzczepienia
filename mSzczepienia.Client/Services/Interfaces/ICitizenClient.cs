using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Client.Services.Interfaces;

public interface ICitizenClient
{
    Task<PagedResult<SimpleCitizenDto>> GetCitizensAsync(GetCitizensQuery query, CancellationToken ct);
}
