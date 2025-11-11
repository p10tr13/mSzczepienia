using mSzczepienia.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.Models
{
    public class Account
    {
        public Guid accountId { get; set; }
        public required string email { get; set; }
        public Role role { get; set; }
        public required string name { get; set; }
        public required string surname { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime dateOfBirth { get; set; }
    }
}
