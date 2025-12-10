namespace mSzczepienia.Shared.DTO.Queries;

public class GetFacilitiesQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
    public int? Id { get; set; }
}
