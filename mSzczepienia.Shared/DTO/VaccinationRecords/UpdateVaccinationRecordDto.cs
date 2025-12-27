
using System.ComponentModel.DataAnnotations;

namespace mSzczepienia.Shared.DTO.VaccinationRecords;

public class UpdateVaccinationRecordDto
{
    [Required(ErrorMessage = "Vaccination date is required")]
    public DateTime? vaccinationDate { get; set; }
    public DateTime? nextVaccinationDate { get; set; }
    public required string doseNumber { get; set; }
    public required string batch { get; set; }
    public string? remarks { get; set; }
}
