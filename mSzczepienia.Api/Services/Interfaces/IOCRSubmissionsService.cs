using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using System.Security.Claims;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IOCRSubmissionsService
{
    Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingForWorkerAsync(ClaimsPrincipal user, SearchOCRSubmissionQueryDto query, CancellationToken ct);
    Task<List<VaccinationEntryDto>> ProcessOcrSubmissionAsync(string firebaseUid, List<IFormFile> files, CancellationToken ct);
}
