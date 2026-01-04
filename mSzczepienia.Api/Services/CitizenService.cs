using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;
using System.Security.Claims;

namespace mSzczepienia.Api.Services
{
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenRepository _citizenRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly IFacilityRepository _facilityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CitizenService> _logger;

        public CitizenService(ICitizenRepository citizenRepository, IAccountRepository accountRepository, IPersonnelRepository personnelRepository,
            IFacilityRepository facilityRepository, IUnitOfWork unitOfWork, ILogger<CitizenService> logger)
        {
            _citizenRepository = citizenRepository;
            _accountRepository = accountRepository;
            _personnelRepository = personnelRepository;
            _facilityRepository = facilityRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task RegisterDeviceTokenAsync(string firebaseUid, string deviceToken, string language, CancellationToken ct)
        {
            await _citizenRepository.RegisterDeviceTokenAsync(firebaseUid, deviceToken, language, ct);
        }

        public async Task<CitizenDetailsDto?> GetCitizenDetailsAsync(string firebaseUid, CancellationToken ct)
        {
            var citizenDetails = await _citizenRepository.GetCitizenDetailsByUidAsync(firebaseUid, ct);

            return citizenDetails;
        }

        public async Task<CitizenDetailsDto?> GetCitizenDetailsByIdAsync(string requesterFirebaseUid, int citizenId, CancellationToken ct)
        {
            _logger.LogInformation("Citizen details for id {CitizenId} requested by {RequesterUid}", citizenId, requesterFirebaseUid);
            var citizenDetails = await _citizenRepository.GetCitizenDetailsByIdAsync(citizenId, ct);
            return citizenDetails;
        }

        public async Task CreateCitizenAsync(string firebaseUid, CitizenDetailsDto dto, CancellationToken ct)
        {
            var existingFacility = await _facilityRepository.GetByNameAsync(dto.facilityName, ct);
            if (existingFacility == null)
            {
                _logger.LogError("Facility not found while creating user account: {FacilityName}", dto.facilityName);
                throw new InvalidOperationException($"Facility has not been found: {dto.facilityName}");
            }

            var account = new Account
            {
                firebaseUid = firebaseUid,
                name = dto.name,
                surname = dto.surname,
                email = dto.email,
                dateOfBirth = dto.dateOfBirth,
                creationDate = DateTime.UtcNow
            };
            await _accountRepository.AddAsync(account, ct);

            var citizen = new Citizen
            {
                pesel = dto.pesel,
                submissionStatus = SubmissionStatus.NotUploaded,
                account = account,
                medicalFacility = existingFacility
            };
            await _citizenRepository.AddAsync(citizen, ct);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<PagedResult<SimpleCitizenDto>> SearchCitizensAsync(ClaimsPrincipal user, GetCitizensQuery query, CancellationToken ct)
        {
            string? firebaseUid = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(firebaseUid))
            {
                firebaseUid = user.FindFirst("user_id")?.Value ?? user.FindFirst("uid")?.Value;
            }

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return new PagedResult<SimpleCitizenDto>();
            }

            _logger.Log(LogLevel.Information, $"User: {firebaseUid}, searches citizens by: {query.ToString()}");
            int? facilityId = null;

            if (query.SearchInWorkersFacilityOnly)
            {
                facilityId = await _personnelRepository.GetFacilityIdForWorkerByFirebaseUidAsync(firebaseUid, ct);
                if (facilityId == null)
                {
                    _logger.Log(LogLevel.Error, "For existing healthcare worker no facility was found.");
                    return new PagedResult<SimpleCitizenDto>();
                }
            }
            return await _citizenRepository.SearchCitizensAsync(query, facilityId, ct);
        }

        public async Task<CitizenSubmissionStatusDto> GetCitizenSubmissionStatusAsync(string firebaseUid, CancellationToken ct)
        {
            return await _citizenRepository.GetCitizenSubmissionStatusAsync(firebaseUid, ct);
        }
    }
}