using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO.VaccinationRecords
{
    public class VaccinationRecordOverviewDto
    {
        public int id { get; set; }
        public required string vaccineName { get; set; }
        public required string manufacturer { get; set; }
        public required DateTime vaccinationDate { get; set; }
        public required int dosesNumber { get; set; }
    }
}
