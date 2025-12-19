using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Vaccines;

namespace mSzczepienia.Api.Services;

public class VaccineService : IVaccineService
{
    private readonly IVaccineRepository _vaccineRepository;

    public VaccineService(IVaccineRepository vaccineRepository)
    {
        _vaccineRepository = vaccineRepository;
    }

    public Task<List<string>> GetManufacturersAutocompleteAsync(string? query, CancellationToken ct)
    {
        return _vaccineRepository.GetManufacturersAutocompleteAsync(query, ct);
    }

    public Task<List<string>> GetNamesAutocompleteAsync(string? query, CancellationToken ct)
    {
        return _vaccineRepository.GetNamesAutocompleteAsync(query, ct);
    }

    public async Task<PagedResult<VaccineDto>> GetVaccinesAsync(GetVaccinesQuery query, CancellationToken ct)
    {
        return await _vaccineRepository.GetVaccinesAsync(query, ct);
    }

    public async Task<VaccineDto?> GetVaccineByIdAsync(int id, CancellationToken ct)
    {
        return await _vaccineRepository.GetVaccineByIdAsync(id, ct);
    }
}
