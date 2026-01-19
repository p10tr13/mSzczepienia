namespace mSzczepienia.Shared.DTO.Queries;

public class GetPersonnelQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int FacilityId { get; set; }
}
