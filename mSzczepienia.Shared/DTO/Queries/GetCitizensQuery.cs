
namespace mSzczepienia.Shared.DTO.Queries;

public class GetCitizensQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool SearchInWorkersFacilityOnly { get; set; } = true;

    // optional
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Pesel { get; set; }

    public override string ToString()
    {
        return $"name: {Name}, surname: {Surname}, pesel: {Pesel}, page number: {PageNumber}, page size: {PageSize}," +
            $" search in workers facility only: {SearchInWorkersFacilityOnly}";
    }
}
