using mSzczepienia.Shared.Enums;

namespace mSzczepienia.Shared.Models
{
    public class Citizen
    {
        public int citizenId { get; set; }
        public Guid accountId { get; set; }
        public Account account { get; set; } = null!;
        public required string pesel { get; set; }
        public int facilityId { get; set; }
        public MedicalFacility medicalFacility { get; set; } = null!;
        public SubmissionStatus submissionStatus { get; set; }

        // Relations
        public ICollection<OcrSubmission> ocrSubmissions { get; set; } = new List<OcrSubmission>();
        public ICollection<VaccinationRecord> vaccinationRecords { get; set; } = new List<VaccinationRecord>();
    }
}
