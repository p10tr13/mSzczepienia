using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using System.Security.Claims;

namespace mSzczepienia.Api.Services;

public class OCRSubmissionsService : IOCRSubmissionsService
{
    private readonly IOCRSubmissionRepository _ocrSubmissionRepository;
    private readonly IPersonnelRepository _personnelRepository;

    public OCRSubmissionsService(IOCRSubmissionRepository ocrSubmissionRepository, IPersonnelRepository personnelRepository)
    {
        _ocrSubmissionRepository = ocrSubmissionRepository;
        _personnelRepository = personnelRepository;
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
}
