using mSzczepienia.Shared.DTO;

namespace mSzczepienia.Api.Services.Interfaces
{
    public interface ICitizenService
    {
        Task<CitizenDetailsDto?> GetCitizenDetailsAsync(string firebaseUid);
        Task CreateCitizenAsync(string firebaseUid, CitizenDetailsDto dto);
    }
}