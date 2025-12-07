using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces
{
    public interface ICitizenRepository
    {
        Task<CitizenDetailsDto?> GetCitizenDetailsByUidAsync(string firebaseUid);
        Task AddCitizenAsync(Citizen citizen);
    }
}