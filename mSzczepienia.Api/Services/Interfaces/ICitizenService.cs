using mSzczepienia.Shared.DTO.Citizens;

namespace mSzczepienia.Api.Services.Interfaces
{
    public interface ICitizenService
    {
        Task<CitizenDetailsDto?> GetCitizenDetailsAsync(string firebaseUid, CancellationToken ct);
        Task CreateCitizenAsync(string firebaseUid, CitizenDetailsDto dto, CancellationToken ct);
    }
}