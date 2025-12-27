using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Vaccines;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IVaccineClient
{
    Task<List<string>> GetManufacturersAutocompleteAsync(string query, CancellationToken ct);
    Task<List<string>> GetNamesAutocompleteAsync(string query, CancellationToken ct);
    Task<PagedResult<VaccineDto>> GetVaccinesAsync(string? Name, string? Manufacturer, string? Disease, int? Id, int Page, int PageSize, CancellationToken ct);
    Task<VaccineDto?> GetVaccineByIdAsync(int id, CancellationToken ct);
    Task<List<VaccineDto>> SearchVaccinesAsync(string term, int maxResults, CancellationToken ct);
}
