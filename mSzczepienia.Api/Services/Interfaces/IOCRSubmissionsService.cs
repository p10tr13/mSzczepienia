using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using System.Net;
using System.Security.Claims;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IOCRSubmissionsService
{
    Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingForWorkerAsync(ClaimsPrincipal user, SearchOCRSubmissionQueryDto query, CancellationToken ct);
    Task<List<VaccinationEntryDto>> ProcessOcrSubmissionAsync(string firebaseUid, List<IFormFile> files, CancellationToken ct);
    Task<OCRSubmissionPhotoReviewDto?> GetSubmissionPhotoReviewAsync(int submissionId, CancellationToken ct);
    Task<bool> RejectSubmissionAsync(string firebaseUid, int submissionId, CancellationToken ct);
    Task<OCRSubmissionEntriesReviewDto?> GetSubmissionEntriesReviewAsync(int submissionId, CancellationToken ct);
    Task ProcessSubmissionReviewAsync(string firebaseUid, ProcessSubmissionReviewDto request, CancellationToken ct);
    Task DeleteOcrSubmissionAsync(string firebaseUid, CancellationToken ct);
    Task UpdateOcrSubmissionWithEntries(string firebaseUid, List<VaccinationEntryDto> entries, CancellationToken ct);
}
