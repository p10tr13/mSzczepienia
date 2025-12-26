
namespace mSzczepienia.Shared.DTO.Citizens;

public class SimpleCitizenDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public required string Pesel { get; set; }
    public required string SubmissionStatus { get; set; }
}
