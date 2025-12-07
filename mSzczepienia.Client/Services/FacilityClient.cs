using mSzczepienia.Client.Services.Interfaces;
using System.Net.Http.Json;
namespace mSzczepienia.Client.Services;

public class FacilityClient : IFacilityClient
{
    private readonly HttpClient _httpClient;

    public FacilityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetCitiesAutocompleteAsync(string query, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"api/facilities/cities-autocomplete?query={query}", ct);
        var facilityNames = await response.Content.ReadFromJsonAsync<List<string>>(cancellationToken: ct);
        return facilityNames ?? new List<string>();
    }
}
