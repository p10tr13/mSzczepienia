using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Api.Models
{
    public class VaccineDisease
    {
        public int vaccineId { get; set; }
        public VaccineDefinition vaccineDefinition { get; set; } = null!;
        public int diseaseId { get; set; }
        public Disease disease { get; set; } = null!;
    }
}
