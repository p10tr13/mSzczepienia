using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IOCRSubmissionRepository
{
    Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingForFacilityAsync(int facilityId, SearchOCRSubmissionQueryDto query, CancellationToken ct);
}
