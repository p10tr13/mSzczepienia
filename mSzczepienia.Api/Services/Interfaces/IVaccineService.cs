namespace mSzczepienia.Api.Services.Interfaces;

public interface IVaccineService
{
    Task<List<string>> GetManufacturersAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<string>> GetNamesAutocompleteAsync(string? query, CancellationToken ct);
}
