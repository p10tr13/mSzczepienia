namespace mSzczepienia.Shared.DTO.Vaccines;

public class VaccineDto
{
    public int VaccineId { get; set; }
    public required string VaccineName { get; set; }
    public required string Manufacturer { get; set; }
    public required List<string> DiseasesEn { get; set; }
    public required List<string> DiseasesPl { get; set; }
}
