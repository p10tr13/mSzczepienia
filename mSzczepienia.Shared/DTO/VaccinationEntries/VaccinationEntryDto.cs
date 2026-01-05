using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO.VaccinationEntries
{
    public class VaccinationEntryDto
    {
        public int Id { get; set; }
        public string? VaccineNameOcr { get; set; }
        public string? DateOcr { get; set; } 
        public string? BatchOcr { get; set; }
        public required string VaccineName { get; set; }
        public required DateTime Date { get; set; }
        public required string Batch { get; set; }
        public required string Status { get; set; }
    }
}
