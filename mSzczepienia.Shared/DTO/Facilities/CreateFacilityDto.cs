
namespace mSzczepienia.Shared.DTO.Facilities;

public class CreateFacilityDto
{
    public string FacilityName { get; set; } = string.Empty;

    public string Nip { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = "Polska";
}
