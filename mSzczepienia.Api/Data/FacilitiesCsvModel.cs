using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Api.Data
{
    public class FacilitiesCsvModel
    {
        public required string facilityName { get; set; }
        public required string street { get; set; }
        public required string postalCode { get; set; }
        public required string city { get; set; }
        public required string country { get; set; }
        public required string nip { get; set; }
        public required string phoneNumber { get; set; }
        [Format("yyyy-MM-dd")]
        public DateTime creationDate { get; set; }
    }

}
