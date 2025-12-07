using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Api.Repositories.Interfaces;

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
}
