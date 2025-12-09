using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Facilities;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IFacilityService
{
    Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<FacilityListDto>> GetFacilityNamesAsync(CancellationToken ct);

}
