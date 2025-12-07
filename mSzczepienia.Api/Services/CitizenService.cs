using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
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

        public async Task<CitizenDetailsDto?> GetCitizenDetailsAsync(string firebaseUid)
        {
            var citizenDetails = await _citizenRepository.GetCitizenDetailsByUidAsync(firebaseUid);

            return citizenDetails;
        }

        public async Task CreateCitizenAsync(string firebaseUid, CitizenDetailsDto dto)
        {
            var newCitizen = new Citizen
            {
                peselHash = dto.peselHash,
                submissionStatus = SubmissionStatus.NotUploaded,
                account = new Account
                {
                    firebaseUid = firebaseUid,
                    name = dto.name,
                    surname = dto.surname,
                    email = dto.email,
                    dateOfBirth = dto.dateOfBirth,
                    creationDate = DateTime.UtcNow
                },

                // TODO
                medicalFacility = new MedicalFacility
                {
                    facilityName = dto.facilityName,
                    address = "Unknown Address", // Placeholder
                    nip = "0000000000" // Placeholder
                }
            };

            await _citizenRepository.AddCitizenAsync(newCitizen);
        }
    }
}