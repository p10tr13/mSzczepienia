using mSzczepienia.Shared.DTO.VaccinationEntries;

namespace mSzczepienia.Shared.DTO.OCRSubmissions;

public class ProcessSubmissionReviewDto
{
    public int SubmissionId { get; set; }
    public List<DetailedVaccinationEntryDto> Entries { get; set; }
}
