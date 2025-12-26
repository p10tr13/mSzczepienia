using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using System.Security.Claims;

namespace mSzczepienia.Api.Services
{
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenRepository _citizenRepository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly ILogger<CitizenService> _logger;

        public CitizenService(ICitizenRepository citizenRepository, IPersonnelRepository personnelRepository, ILogger<CitizenService> logger)
        {
            _citizenRepository = citizenRepository;
            _personnelRepository = personnelRepository;
            _logger = logger;
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
            await _citizenRepository.AddCitizenAsync(firebaseUid, dto, ct);
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
    }
}