using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Api.Data
{
    public class VaccineCsvModel { 
        public required string vaccineName { get; set; } 
        public required string manufacturer { get; set; }
        public required string vaccineDescriptionEn { get; set; }
        public required string vaccineDescriptionPl { get; set; }

        [Format("dd/MM/yyyy")]
        public DateTime creationDate { get; set; } 
    }
}
