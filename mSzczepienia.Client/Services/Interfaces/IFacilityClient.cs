namespace mSzczepienia.Client.Services.Interfaces;

public interface IFacilityClient
{
    Task<List<string>> GetCitiesAutocompleteAsync(string query, CancellationToken ct);
}
