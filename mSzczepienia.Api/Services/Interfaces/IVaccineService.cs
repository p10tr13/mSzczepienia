using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IVaccineService
{
    Task<List<string>> GetManufacturersAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<string>> GetNamesAutocompleteAsync(string? query, CancellationToken ct);
    Task<PagedResult<VaccineDto>> GetVaccinesAsync(GetVaccinesQuery query, CancellationToken ct);
    Task<VaccineDto?> GetVaccineByIdAsync(int id, CancellationToken ct);
    Task CreateVaccineAsync(CreateVaccineDto dto, CancellationToken ct);
    Task<DetailedVaccineDto?> GetDetailedVaccineByIdAsync(int id, CancellationToken ct);
    Task UpdateVaccineAsync(int id, CreateVaccineDto dto, CancellationToken ct);
}
