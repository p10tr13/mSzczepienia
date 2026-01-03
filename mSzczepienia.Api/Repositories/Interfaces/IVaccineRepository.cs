using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Vaccines;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IVaccineRepository
{
    Task<List<string>> GetNamesAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<string>> GetManufacturersAutocompleteAsync(string? query, CancellationToken ct);
    Task<PagedResult<VaccineDto>> GetVaccinesAsync(GetVaccinesQuery query, CancellationToken ct);
    Task<VaccineDto?> GetVaccineByIdAsync(int id, CancellationToken ct);
    Task<List<VaccineDto>> GetVaccinesByIdsAsync(List<int> vaccineIds, CancellationToken ct);
}
