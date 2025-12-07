using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO
{
    public class CitizenDetailsDto
    {
        public required string name { get; set; }
        public required string surname { get; set; }
        public required string email { get; set; }
        public required DateTime dateOfBirth { get; set; }
        public required DateTime creationDate { get; set; }
        public required string peselHash { get; set; }
        public required string facilityName { get; set; }
        public required string submissionStatus { get; set; }
    }
}
