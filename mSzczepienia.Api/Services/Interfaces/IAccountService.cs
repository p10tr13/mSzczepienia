using mSzczepienia.Shared.DTO.Accounts;

namespace mSzczepienia.Api.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ProfileDto?> GetProfileAsync(string firebaseUid, CancellationToken ct);
    }
}
