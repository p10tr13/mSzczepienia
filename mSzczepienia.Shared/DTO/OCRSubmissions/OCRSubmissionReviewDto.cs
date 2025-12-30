
namespace mSzczepienia.Shared.DTO.OCRSubmissions;

public class OCRSubmissionReviewDto
{
    public int SubmissionId { get; set; }
    public DateTime SubmissionDate { get; set; }
    public string PatientFirstName { get; set; }
    public string PatientSurame { get; set; }
    public string Pesel { get; set; }
    public DateTime PatientDateOfBirth { get; set; }
    public List<string> Photos { get; set; }
    public string MedicalFacilityName { get; set; }
    public string MedicalFacilityStreet { get; set; }
    public string MedicalFacilityCity { get; set; }
    public string MedicalFacilityPostalCode { get; set; }
}
