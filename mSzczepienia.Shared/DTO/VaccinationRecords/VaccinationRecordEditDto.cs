
namespace mSzczepienia.Shared.DTO.VaccinationRecords;

public class VaccinationRecordEditDto
{
    public int recordId { get; set; }

    // citizen info
    public int citizenId { get; set; }
    public required string name { get; set; }
    public required string surname { get; set; }
    public required string pesel { get; set; }
    public required DateTime dateOfBirth { get; set; }

    // vaccine info
    public int vaccineId { get; set; }
    public required string vaccineName { get; set; }
    public required string manufacturer { get; set; }

    // record fields
    public required DateTime vaccinationDate { get; set; }
    public DateTime? nextVaccinationDate { get; set; }
    public required string doseNumber { get; set; }
    public required string batch { get; set; }
    public string? remarks { get; set; }
}