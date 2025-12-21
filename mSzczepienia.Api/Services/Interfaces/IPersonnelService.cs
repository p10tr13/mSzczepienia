using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Personnel;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IPersonnelService
{
    Task<PagedResult<PersonnelDto>> GetPersonnelAsync(GetPersonnelQuery query, CancellationToken ct);
}
