namespace mSzczepienia.Shared.Models
{
    public class VaccinationEntry
    {
        public int vaccinationEntryId { get; set; }
        public VaccinationRecord? vaccinationRecord { get; set; } = null!; // At this point record might not exist yet
        public int ocrSubmissionId { get; set; }
        public OcrSubmission ocrSubmission { get; set; } = null!;
        public required string batchOcr { get; set; }
        public required string vaccineNameOcr { get; set; }
        public required string dateOcr { get; set; }
        public int? vaccineId { get; set; }
        public VaccineDefinition? vaccineDefinition { get; set; } = null!;
        public string? batch { get; set; }
        public DateTime? date { get; set; }
    }
}
