
namespace mSzczepienia.Shared.DTO.VaccinationRecords;

public class VaccinationRecordSimpleListItemDto
{
    public int RecordId { get; set; }
    public required string VaccineName { get; set; }
    public required string Manufacturer { get; set; }
    public required DateTime VaccinationDate { get; set; }
}
