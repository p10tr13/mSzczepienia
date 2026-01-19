
namespace mSzczepienia.Shared.DTO.VaccinationRecords;

public class VaccinationRecordOverviewDto
{
    public int id { get; set; }
    public required string vaccineName { get; set; }
    public required string shortDescriptionEn { get; set; }
    public required string shortDescriptionPl { get; set; }
    public required DateTime vaccinationDate { get; set; }
    public required int dosesNumber { get; set; }
}
