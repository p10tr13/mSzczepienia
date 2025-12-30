using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO.VaccinationRecords
{
    public class VaccinationRecordDetailsDto
    {
        public int vaccineId { get; set; }
        public required string vaccineName { get; set; }
        public required string manufacturer { get; set; }
        public required string vaccineDescriptionPl { get; set; }
        public required string vaccineDescriptionEn { get; set; }
        public required string shortDescriptionPl { get; set; }
        public required string shortDescriptionEn { get; set; }
        public required DateTime creationDate { get; set; }
        public List<string> diseasesEn { get; set; } = new List<string>();
        public List<string> diseasesPl { get; set; } = new List<string>();
        public required DateTime vaccinationDate { get; set; }
        public DateTime? nextVaccinationDate { get; set; }
        public required string doseNumber { get; set; }
        public required string batch { get; set; }
        public string? remarks { get; set; }
    }
}
