using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using System.Security.Claims;

namespace mSzczepienia.Api.Services;

public class OCRSubmissionsService : IOCRSubmissionsService
{
    private readonly IOCRSubmissionRepository _ocrSubmissionRepository;
    private readonly IPersonnelRepository _personnelRepository;
    private readonly ICitizenRepository _citizenRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IBlobService _blobService;

    public OCRSubmissionsService(IOCRSubmissionRepository ocrSubmissionRepository, IPersonnelRepository personnelRepository, ICitizenRepository citizenRepository, IFacilityRepository facilityRepository, IBlobService blobService)
    {
        _ocrSubmissionRepository = ocrSubmissionRepository;
        _personnelRepository = personnelRepository;
        _citizenRepository = citizenRepository;
        _facilityRepository = facilityRepository;
        _blobService = blobService;
    }

    public async Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingForWorkerAsync(ClaimsPrincipal user, SearchOCRSubmissionQueryDto query, CancellationToken ct)
    {
        string? firebaseUid = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            firebaseUid = user.FindFirst("user_id")?.Value ?? user.FindFirst("uid")?.Value;
        }

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return new PagedResult<SearchOCRSubmissionDTO>();
        }

        // If worker requested all submissions (not limited to their facility), return global pending list
        if (query != null && query.SearchInWorkersFacilityOnly == false)
        {
            return await _ocrSubmissionRepository.GetPendingAsync(query, ct);
        }

        var facilityId = await _personnelRepository.GetFacilityIdForWorkerByFirebaseUidAsync(firebaseUid, ct);
        if (!facilityId.HasValue)
        {
            return new PagedResult<SearchOCRSubmissionDTO>();
        }

        return await _ocrSubmissionRepository.GetPendingForFacilityAsync(facilityId.Value, query, ct);
    }

    public async Task<List<VaccinationEntryDto>> ProcessOcrSubmissionAsync(string firebaseUid, List<IFormFile> files, CancellationToken ct)
    {
        return await _ocrSubmissionRepository.ProcessOcrSubmissionAsync(firebaseUid, files, ct);
    }

    public async Task<OCRSubmissionReviewDto?> GetSubmissionReviewAsync(int submissionId, CancellationToken ct)
    {
        var submission = await _ocrSubmissionRepository.GetByIdAsync(submissionId, ct);
        if (submission == null)
            return null;

        var citizenDetails = await _citizenRepository.GetCitizenDetailsByIdAsync(submission.citizenId, ct);

        int? facilityId = await _citizenRepository.GetFacilityIdByCitizenIdAsync(submission.citizenId, ct);
        FacilityDto? facility = null;
        if (facilityId.HasValue)
        {
            facility = await _facilityRepository.GetFacilityByIdAsync(facilityId.Value, ct);
        }

        List<string> photos = new List<string>();
        try
        {
            photos = await _blobService.GetSasUrlsForDirectoryAsync(submission.photoUrl);
        }
        catch
        {
            photos = new List<string>();
        }

        var dto = new OCRSubmissionReviewDto
        {
            SubmissionId = submission.submissionId,
            SubmissionDate = submission.submissionDate,
            PatientFirstName = citizenDetails?.name ?? string.Empty,
            PatientSurame = citizenDetails?.surname ?? string.Empty,
            Pesel = citizenDetails?.pesel ?? string.Empty,
            PatientDateOfBirth = citizenDetails?.dateOfBirth ?? DateTime.MinValue,
            Photos = photos,
            MedicalFacilityName = facility?.Name ?? citizenDetails?.facilityName ?? string.Empty,
            MedicalFacilityStreet = facility?.Street ?? string.Empty,
            MedicalFacilityCity = facility?.City ?? string.Empty,
            MedicalFacilityPostalCode = facility?.PostalCode ?? string.Empty
        };

        return dto;
    }

    public async Task<bool> RejectSubmissionAsync(int submissionId, CancellationToken ct)
    {
        var result = await _ocrSubmissionRepository.RejectSubmissionAsync(submissionId, ct);
        return result;
    }
}
