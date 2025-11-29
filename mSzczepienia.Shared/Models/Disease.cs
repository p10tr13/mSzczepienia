using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.Models
{
    public class Disease
    {
        public int diseaseId { get; set; }
        public required string diseaseNameEn { get; set; }
        public required string diseaseNamePl { get; set; }

        // Relations
        public ICollection<VaccineDisease> vaccineDiseases { get; set; } = new List<VaccineDisease>();
    }
}
