namespace mSzczepienia.Shared.DTO.Vaccines;

public class VaccineDto
{
    public int VaccineId { get; set; }
    public required string VaccineName { get; set; }
    public required string Manufacturer { get; set; }
    public required List<string> DiseasesEn { get; set; }
    public required List<string> DiseasesPl { get; set; }

    public override string ToString()
    {
        var diseases = (DiseasesEn != null && DiseasesEn.Count > 0) ? string.Join(", ", DiseasesEn) : "";
        return $"{VaccineName} ({Manufacturer}) {diseases}";
    }
}
