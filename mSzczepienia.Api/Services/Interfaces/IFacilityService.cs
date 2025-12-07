namespace mSzczepienia.Api.Services.Interfaces;

public interface IFacilityService
{
    Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct);
}
