using System.ComponentModel.DataAnnotations;

namespace mSzczepienia.Shared.Models
{
    public class MedicalFacility
    {
        public int facilityId { get; set; }
        public required string facilityName { get; set; }
        public required string city { get; set; }
        public required string street { get; set; }
        public required string country { get; set; }
        public required string nip { get; set; }
        [MaxLength(10)]
        public required string postalCode { get; set; }
        public DateTime creationDate { get; set; }

        // Relations
        public ICollection<Citizen> citizens { get; set; } = new List<Citizen>();
        public ICollection<Personnel> personnels { get; set; } = new List<Personnel>();
    }
}
