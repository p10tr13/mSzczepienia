using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Queries;
using System.Security.Claims;

namespace mSzczepienia.Api.Services.Interfaces
{
    public interface ICitizenService
    {
        Task<CitizenDetailsDto?> GetCitizenDetailsAsync(string firebaseUid, CancellationToken ct);
        Task<CitizenDetailsDto?> GetCitizenDetailsByIdAsync(string requesterFirebaseUid, int citizenId, CancellationToken ct);
        Task CreateCitizenAsync(string firebaseUid, CitizenDetailsDto dto, CancellationToken ct);
        Task<PagedResult<SimpleCitizenDto>> SearchCitizensAsync(ClaimsPrincipal user, GetCitizensQuery query, CancellationToken ct);
        Task<CitizenSubmissionStatusDto> GetCitizenSubmissionStatusAsync(string firebaseUid, CancellationToken ct);
    }
}