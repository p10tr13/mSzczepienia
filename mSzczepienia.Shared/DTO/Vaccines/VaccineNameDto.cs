using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO.Vaccines
{
    public class VaccineNameDto
    {
        public int VaccineId { get; set; }
        public string VaccineName { get; set; } = null!;
    }
}
