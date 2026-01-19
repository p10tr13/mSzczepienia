namespace mSzczepienia.Shared.Models
{
    public class VaccineDisease
    {
        public int vaccineId { get; set; }
        public VaccineDefinition vaccineDefinition { get; set; } = null!;
        public int diseaseId { get; set; }
        public Disease disease { get; set; } = null!;
    }
}
