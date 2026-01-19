using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IFacilityRepository
{
    Task<MedicalFacility?> GetByNameAsync(string name, CancellationToken ct);
    Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<FacilityListDto>> GetFacilityNamesAutocompleteAsync(string? query, CancellationToken ct);
    Task<PagedResult<MedicalFacility>> SearchFacilitiesAsync(GetFacilitiesQuery query, CancellationToken ct);
    Task<MedicalFacility?> GetByIdAsNoTrackingAsync(int id, CancellationToken ct);
    Task AddAsync(MedicalFacility facility, CancellationToken ct);
    Task<MedicalFacility?> GetByIdAsync(int id, CancellationToken ct);
    void Update(MedicalFacility facility);
}
