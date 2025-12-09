using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Services
{
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenRepository _citizenRepository;

        public CitizenService(ICitizenRepository citizenRepository)
        {
            _citizenRepository = citizenRepository;
        }

        public async Task<CitizenDetailsDto?> GetCitizenDetailsAsync(string firebaseUid, CancellationToken ct)
        {
            var citizenDetails = await _citizenRepository.GetCitizenDetailsByUidAsync(firebaseUid, ct);

            return citizenDetails;
        }

        public async Task CreateCitizenAsync(string firebaseUid, CitizenDetailsDto dto, CancellationToken ct)
        {
            await _citizenRepository.AddCitizenAsync(firebaseUid, dto, ct);
        }
    }
}