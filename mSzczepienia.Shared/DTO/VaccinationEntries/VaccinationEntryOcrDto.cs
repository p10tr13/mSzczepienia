using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO.VaccinationEntries
{
    public class VaccinationEntryOcrDto
    {
        public string? vaccineName { get; set; }
        public string? vaccinationDate { get; set; }
        public string? batch { get; set; }
    }
}
