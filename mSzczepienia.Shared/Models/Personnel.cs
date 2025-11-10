using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSzczepienia.Shared.Models
{
    public class Personnel
    {
        public int personnelId { get; set; }
        public Guid accountId { get; set; }
        public Account account { get; set; } = null!;
        public int facilityId { get; set; }
        public MedicalFacility medicalFacility { get; set; } = null!;
        public required string pwzNumber { get; set; }
        public required string position { get; set; }

        // Relations
        public ICollection<OcrSubmission> ocrSubmissions { get; set; } = new List<OcrSubmission>();
    }
}
