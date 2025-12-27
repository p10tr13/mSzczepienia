using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Accounts;

namespace mSzczepienia.Api.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountProfileRepository _profileRepository;

        public AccountService(IAccountProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<ProfileDto?> GetProfileAsync(string firebaseUid, CancellationToken ct)
        {
            return await _profileRepository.GetProfileByFirebaseUidAsync(firebaseUid, ct);
        }
    }
}
