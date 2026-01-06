
namespace mSzczepienia.Shared.DTO.Queries;

public class SearchAdminQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;

    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
}
