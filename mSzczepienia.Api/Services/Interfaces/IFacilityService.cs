using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IFacilityService
{
    Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<FacilityListDto>> GetFacilityNamesAutocompleteAsync(string? query, CancellationToken ct);
    Task<PagedResult<SearchFacilityDto>> SearchFacilitiesAsync(GetFacilitiesQuery query, CancellationToken ct);
    Task<FacilityDto?> GetFacilityByIdAsync(int id, CancellationToken ct);
    Task<int> CreateFacilityAsync(CreateFacilityDto dto, CancellationToken ct);
    Task UpdateFacilityAsync(int id, CreateFacilityDto dto, CancellationToken ct);
}
