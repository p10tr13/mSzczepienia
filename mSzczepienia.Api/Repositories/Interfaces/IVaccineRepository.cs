namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IVaccineRepository
{
    Task<List<string>> GetNamesAutocompleteAsync(string? query, CancellationToken ct);
    Task<List<string>> GetManufacturersAutocompleteAsync(string? query, CancellationToken ct);
}
