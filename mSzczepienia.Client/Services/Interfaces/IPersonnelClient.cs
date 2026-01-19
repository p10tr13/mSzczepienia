using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IPersonnelClient
{
    Task<PagedResult<PersonnelDto>> GetPersonnelByFacilityAsync(int facilityId, int pageIndex, int pageSize, CancellationToken ct);
    Task<PagedResult<PersonnelDto>> SearchPersonnelAsync(SearchPersonnelQuery query, CancellationToken ct);
    Task<DetailedPersonnelDto?> GetPersonnelByIdAsync(int personnelId, CancellationToken ct);
    Task UpdatePersonnelAsync(int personnelId, UpdatePersonnelDto updatePersonnelDto, CancellationToken ct);
}
