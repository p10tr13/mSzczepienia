namespace mSzczepienia.Shared.Models
{
    public class VaccinationRecord
    {
        public int recordId { get; set; }
        public int citizenId { get; set; }
        public Citizen citizen { get; set; } = null!;
        public int vaccineId { get; set; }
        public VaccineDefinition vaccineDefinition { get; set; } = null!;
        public DateTime vaccinationDate { get; set; }
        public required string doseNumber { get; set; }
        public DateTime? nextVaccinationDate { get; set; }
        public required string batch { get; set; }
        public int? sourceEntryId { get; set; }
        public VaccinationEntry? sourceEntry { get; set; } = null!;
        public string? remarks { get; set; }
        public DateTime creationDate { get; set; }
    }
}
