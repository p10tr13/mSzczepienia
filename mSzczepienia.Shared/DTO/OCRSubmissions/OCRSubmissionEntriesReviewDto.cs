using mSzczepienia.Shared.DTO.VaccinationEntries;

namespace mSzczepienia.Shared.DTO.OCRSubmissions;

public class OCRSubmissionEntriesReviewDto
{
    public int SubmissionId { get; set; }
    public List<string> Photos { get; set; }
    public List<DetailedVaccinationEntryDto> VaccinationEntries { get; set; }
}
