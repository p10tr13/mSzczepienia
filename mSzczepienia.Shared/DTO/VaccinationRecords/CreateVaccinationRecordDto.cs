
namespace mSzczepienia.Shared.DTO.VaccinationRecords;

public class CreateVaccinationRecordDto
{
    public int citizenId { get; set; }
    public int vaccineId { get; set; }
    public DateTime vaccinationDate { get; set; }
    public required string doseNumber { get; set; }
    public DateTime? nextVaccinationDate { get; set; }
    public required string batch { get; set; }
    public int? sourceEntryId { get; set; }
    public string? remarks { get; set; }
}