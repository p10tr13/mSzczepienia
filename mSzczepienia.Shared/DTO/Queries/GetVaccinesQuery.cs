namespace mSzczepienia.Shared.DTO.Queries;

public class GetVaccinesQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SearchPhrase { get; set; }

    public string? Name { get; set; }
    public string? Manufacturer { get; set; }
    public string? Disease { get; set; }
    public int? Id { get; set; }
}
