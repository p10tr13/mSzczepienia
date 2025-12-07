namespace mSzczepienia.Client.Services.Interfaces;

public interface IVaccineClient
{
    Task<List<string>> GetManufacturersAutocompleteAsync(string query, CancellationToken ct);
    Task<List<string>> GetNamesAutocompleteAsync(string query, CancellationToken ct);
}
