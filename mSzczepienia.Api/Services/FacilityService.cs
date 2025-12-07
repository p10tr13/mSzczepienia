using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;

namespace mSzczepienia.Api.Services;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;

    public FacilityService(IFacilityRepository facilityRepository)
    {
        _facilityRepository = facilityRepository;
    }

    public async Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct)
    {
        return await _facilityRepository.GetCitiesAutocompleteAsync(query, ct);
    }
}
