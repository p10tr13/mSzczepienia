using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IFacilityRepository
{
    Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<FacilityListDto>> GetFacilityNamesAsync(CancellationToken ct);
    Task<PagedResult<MedicalFacility>> SearchFacilitiesAsync(GetFacilitiesQuery query, CancellationToken ct);
}
