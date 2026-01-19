using mSzczepienia.Shared.DTO.Diseases;

namespace mSzczepienia.Shared.DTO.Vaccines;

public class CreateVaccineDto
{
    public required string VaccineName { get; set; }
    public required string Manufacturer { get; set; }

    public required string VaccineDescriptionEn { get; set; }
    public required string VaccineDescriptionPl { get; set; }
    public required string ShortDescriptionEn { get; set; }
    public required string ShortDescriptionPl { get; set; }

    public List<DiseaseDto> Diseases { get; set; } = new();
}
