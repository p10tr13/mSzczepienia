using mSzczepienia.Api.Repositories;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Facilities;

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

    public async Task<List<FacilityListDto>> GetFacilityNamesAsync(CancellationToken ct)
    {
        var facilityNames = await _facilityRepository.GetFacilityNamesAsync(ct);

        return facilityNames;
    }
}
