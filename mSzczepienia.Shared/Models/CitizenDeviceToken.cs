using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.Models
{
    public class CitizenDeviceToken
    {
        public int citizenDeviceTokenId { get; set; }
        public int citizenId { get; set; }
        public Citizen citizen { get; set; } = null!;
        public required string deviceToken { get; set; }
        public required string language { get; set; }
        public DateTime lastUpdated { get; set; }
    }
}
