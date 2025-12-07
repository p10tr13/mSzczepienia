using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO
{
    public class MedicalFacilityDto
    {
        public required int facilityId { get; set; }
        public required string facilityName { get; set; }
    }
}
