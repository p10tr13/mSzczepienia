using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IOCRSubmissionClient
{
    Task<PagedResult<SearchOCRSubmissionDTO>> GetDashboardOCRSubmissions(int PageSize, CancellationToken ct);
    Task<PagedResult<SearchOCRSubmissionDTO>> GetOCRSubmissionsAsync(SearchOCRSubmissionQueryDto query, CancellationToken ct);
}
