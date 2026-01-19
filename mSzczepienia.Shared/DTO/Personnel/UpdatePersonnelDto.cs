using mSzczepienia.Shared.Enums;

namespace mSzczepienia.Shared.DTO.Personnel;

public class UpdatePersonnelDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PwzNumber { get; set; } = string.Empty;
    public Position Position { get; set; }
}
