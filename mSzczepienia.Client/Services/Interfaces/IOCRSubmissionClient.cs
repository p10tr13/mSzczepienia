using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IOCRSubmissionClient
{
    Task<PagedResult<SearchOCRSubmissionDTO>> GetDashboardOCRSubmissions(int PageSize, CancellationToken ct);
    Task<PagedResult<SearchOCRSubmissionDTO>> GetOCRSubmissionsAsync(SearchOCRSubmissionQueryDto query, CancellationToken ct);
    Task<OCRSubmissionPhotoReviewDto?> GetSubmissionPhotoReviewAsync(int submissionId, CancellationToken ct);
    Task RejectSubmissionAsync(int submissionId, CancellationToken ct);
    Task<OCRSubmissionEntriesReviewDto?> GetSubmissionEntriesReviewAsync(int submissionId, CancellationToken ct);
    Task ReviewSubmissionAsync(int submissionId, List<DetailedVaccinationEntryDto> entries, CancellationToken ct);
}
