using System.ComponentModel.DataAnnotations;
using mSzczepienia.Shared.Enums;

namespace mSzczepienia.Shared.DTO.Users;

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Hasło musi mieć minimum 6 znaków")]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public DateTime? DateOfBirth { get; set; } = DateTime.Today.AddYears(-30);

    [Required]
    public Role Role { get; set; } = Role.HealthWorker;

    public string? PwzNumber { get; set; }
    public int? FacilityId { get; set; }
    public Position? Position { get; set; }
}
