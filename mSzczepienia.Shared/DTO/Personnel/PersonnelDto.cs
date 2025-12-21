namespace mSzczepienia.Shared.DTO.Personnel;

public class PersonnelDto
{
    public int Id { get; set; }
    public Guid AccountId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int FacilityId { get; set; }
    public string PwzNumber { get; set; }
    public string Position { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}
