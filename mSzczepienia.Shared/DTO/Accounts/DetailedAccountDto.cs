using mSzczepienia.Shared.Enums;

namespace mSzczepienia.Shared.DTO.Accounts;

public class DetailedAccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Role Role { get; set; }
}
