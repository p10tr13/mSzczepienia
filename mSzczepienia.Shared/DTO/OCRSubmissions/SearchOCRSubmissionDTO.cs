
namespace mSzczepienia.Shared.DTO.OCRSubmissions;

public class SearchOCRSubmissionDTO
{
    public required int ID { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required DateTime SubmissionDate { get; set; }
    public required string Pesel { get; set; }
}
