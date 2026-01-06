using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IFacilityClient
{
    Task<List<string>> GetCitiesAutocompleteAsync(string query, CancellationToken ct);
    Task<PagedResult<SearchFacilityDto>> GetFacilitiesAsync(string? City, string? Street, string? PostalCode, int? Id, int Page, int PageSize, CancellationToken ct);
    Task<PagedResult<SearchFacilityDto>> GetFacilitiesAsync(GetFacilitiesQuery query, CancellationToken ct);
    Task<FacilityDto> GetFacilityByIdAsync(int id, CancellationToken ct);
    Task<int> CreateFacilityAsync(CreateFacilityDto dto, CancellationToken ct);
    Task UpdateFacilityAsync(int id, CreateFacilityDto dto, CancellationToken ct);
}
