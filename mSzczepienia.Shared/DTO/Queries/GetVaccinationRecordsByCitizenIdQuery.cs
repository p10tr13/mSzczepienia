namespace mSzczepienia.Shared.DTO.Queries;

public class GetVaccinationRecordsByCitizenIdQuery
{
    public int CitizenId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortingField { get; set; }
    public string? SortingDirection { get; set; }
}