using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IOCRSubmissionClient
{
    Task<PagedResult<SearchOCRSubmissionDTO>> GetDashboardOCRSubmissions(int PageSize, CancellationToken ct);
}
