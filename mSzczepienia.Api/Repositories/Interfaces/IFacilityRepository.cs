using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Facilities;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IFacilityRepository
{
    Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<FacilityListDto>> GetFacilityNamesAsync(CancellationToken ct);

    //Task<PagedResult<SearchFacilityDto>> SearchFacilitiesAsync(SearchFacilityRequest request, int page, int pageSize, CancellationToken ct);
}
