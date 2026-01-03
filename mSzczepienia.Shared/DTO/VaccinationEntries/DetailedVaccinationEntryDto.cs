using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Shared.Enums;

namespace mSzczepienia.Shared.DTO.VaccinationEntries;

public class DetailedVaccinationEntryDto
{
    public int Id { get; set; }
    public required string VaccineNameOcr { get; set; }
    public required string DateOcr { get; set; }
    public required string BatchOcr { get; set; }
    public VaccineDto? Vaccine { get; set; } = null!;
    public DateTime? Date { get; set; }
    public string? Batch { get; set; }
    public VaccinationEntryStatus Status { get; set; }
}
