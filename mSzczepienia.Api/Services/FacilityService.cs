using mSzczepienia.Api.Repositories;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;

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

    public async Task<PagedResult<SearchFacilityDto>> SearchFacilitiesAsync(GetFacilitiesQuery query, CancellationToken ct)
    {
        var pagedFacilities = await _facilityRepository.SearchFacilitiesAsync(query, ct);

        var dtos = pagedFacilities.Items.Select(f => new SearchFacilityDto
        {
            Id = f.facilityId,
            Name = f.facilityName,
            Street = f.street,
            City = f.city,
            PostalCode = f.postalCode
        }).ToList();

        return new PagedResult<SearchFacilityDto>
        {
            Items = dtos,
            TotalCount = pagedFacilities.TotalCount,
            PageNumber = pagedFacilities.PageNumber,
            PageSize = pagedFacilities.PageSize
        };
    }
}
