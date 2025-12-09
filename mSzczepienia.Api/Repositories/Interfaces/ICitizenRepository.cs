using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces
{
    public interface ICitizenRepository
    {
        Task<CitizenDetailsDto?> GetCitizenDetailsByUidAsync(string firebaseUid, CancellationToken ct);
        Task AddCitizenAsync(string firebaseUid, CitizenDetailsDto dto, CancellationToken ct);
    }
}