using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IFacilityClient
{
    Task<List<string>> GetCitiesAutocompleteAsync(string query, CancellationToken ct);
    Task<PagedResult<SearchFacilityDto>> GetFacilitiesAsync(string? City, string? Street, string? PostalCode, int? Id, int Page, int PageSize, CancellationToken ct);
}
