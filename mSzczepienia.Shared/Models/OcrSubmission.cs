using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.Models
{
    public class OcrSubmission
    {
        public int submissionId { get; set; }
        public int citizenId { get; set; }
        public Citizen citizen { get; set; } = null!;
        public required string photoUrl { get; set; }
        public required string status { get; set; }
        public int? verifierId { get; set; }
        public Personnel? verifier { get; set; } = null!;
        public DateTime submissionDate { get; set; }
        public string? verifierNotes { get; set; }
        public DateTime? verificationDate { get; set; }

        // Relations
        public ICollection<VaccinationEntry> vaccinationEntries { get; set; } = new List<VaccinationEntry>();
    }
}
