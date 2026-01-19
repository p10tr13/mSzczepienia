using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces
{
    public interface ICitizenRepository
    {
        Task RegisterDeviceTokenAsync(string firebaseUid, string deviceToken, string language, CancellationToken ct);
        Task<CitizenDetailsDto?> GetCitizenDetailsByUidAsync(string firebaseUid, CancellationToken ct);
        Task<CitizenDetailsDto?> GetCitizenDetailsByIdAsync(int citizenId, CancellationToken ct);
        Task<int?> GetFacilityIdByCitizenIdAsync(int citizenId, CancellationToken ct);
        Task<PagedResult<SimpleCitizenDto>> SearchCitizensAsync(GetCitizensQuery query, int? facilityId, CancellationToken ct);
        Task<CitizenSubmissionStatusDto> GetCitizenSubmissionStatusAsync(string firebaseUid, CancellationToken ct);
        Task<Citizen?> GetByIdAsync(int citizenId, CancellationToken ct);
        Task<Citizen?> GetByIdNoTrackingAsync(int citizenId, CancellationToken ct);
        Task AddAsync(Citizen citizen, CancellationToken ct);
        void Update(Citizen citizen);
    }
}