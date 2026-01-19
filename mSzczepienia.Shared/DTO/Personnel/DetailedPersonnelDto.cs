using mSzczepienia.Shared.Enums;

namespace mSzczepienia.Shared.DTO.Personnel;

public class DetailedPersonnelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Role Role { get; set; }
    public string PwzNumber { get; set; } = string.Empty;
    public Position Position { get; set; }
    public int FacilityId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string FacilityCity { get; set; } = string.Empty;
    public string FacilityStreet { get; set; } = string.Empty;
    public string FacilityPostalCode { get; set; } = string.Empty;
}
