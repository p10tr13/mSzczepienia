
namespace mSzczepienia.Shared.DTO.Queries;

public class SearchPersonnelQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;

    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }

    public string? FacilityName { get; set; }
    public string? FacilityCity { get; set; }
    public string? FacilityStreet { get; set; }
    public string? FacilityPostalCode { get; set; }
}
