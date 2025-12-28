using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IOCRSubmissionRepository
{
    Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingForFacilityAsync(int facilityId, SearchOCRSubmissionQueryDto query, CancellationToken ct);
    Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingAsync(SearchOCRSubmissionQueryDto query, CancellationToken ct);
    Task<List<VaccinationEntryDto>> ProcessOcrSubmissionAsync(string firebaseUid, List<IFormFile> files, CancellationToken ct);
}
