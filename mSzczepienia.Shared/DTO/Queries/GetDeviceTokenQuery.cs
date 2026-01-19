using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.DTO.Queries
{
    public class GetDeviceTokenQuery
    {
        public required string deviceToken {  get; set; }
        public required string language { get; set; }
    }
}
