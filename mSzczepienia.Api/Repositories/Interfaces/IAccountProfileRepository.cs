using mSzczepienia.Shared.DTO.Accounts;

namespace mSzczepienia.Api.Repositories.Interfaces
{
    public interface IAccountProfileRepository
    {
        Task<ProfileDto?> GetProfileByFirebaseUidAsync(string firebaseUid, CancellationToken ct);
    }
}
