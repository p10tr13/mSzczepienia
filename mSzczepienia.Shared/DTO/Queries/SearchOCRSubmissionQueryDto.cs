
namespace mSzczepienia.Shared.DTO.Queries;

public class SearchOCRSubmissionQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;

    // optional filters
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string? Pesel { get; set; }
}
