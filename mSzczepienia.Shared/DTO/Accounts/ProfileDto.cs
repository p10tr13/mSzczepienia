
namespace mSzczepienia.Shared.DTO.Accounts;

public class ProfileDto
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreationDate { get; set; }
    public string? Role { get; set; }

    // Worker-specific
    public string? PwzNumber { get; set; }
    public string? Position { get; set; }
    public string? FacilityName { get; set; }
}
