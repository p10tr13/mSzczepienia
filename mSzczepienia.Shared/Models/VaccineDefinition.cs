using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.Models
{
    public class VaccineDefinition
    {
        public int vaccineId { get; set; }
        public required string vaccineName { get; set; }
        public required string manufacturer { get; set; }
        public DateTime creationDate { get; set; }

        // Relations
        public ICollection<VaccinationEntry> vaccinationEntries { get; set; } = new List<VaccinationEntry>();
        public ICollection<VaccinationRecord> vaccinationRecords { get; set; } = new List<VaccinationRecord>();
        public ICollection<VaccineDisease> vaccineDiseases { get; set; } = new List<VaccineDisease>();
    }
}
