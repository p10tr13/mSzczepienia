using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Repositories.Interfaces
{
    public interface ICitizenRepository
    {
        Task<CitizenDetailsDto?> GetCitizenDetailsByUidAsync(string firebaseUid, CancellationToken ct);
        Task AddCitizenAsync(string firebaseUid, CitizenDetailsDto dto, CancellationToken ct);
        Task<PagedResult<SimpleCitizenDto>> SearchCitizensAsync(GetCitizensQuery query, int? facilityId, CancellationToken ct);
    }
}