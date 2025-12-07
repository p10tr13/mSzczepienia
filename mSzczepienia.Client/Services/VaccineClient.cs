using mSzczepienia.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace mSzczepienia.Client.Services;

public class VaccineClient : IVaccineClient
{
    private readonly HttpClient _httpClient;

    public VaccineClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetManufacturersAutocompleteAsync(string query, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"api/vaccines/manufacturers-autocomplete?query={query}", ct);
        return await response.Content.ReadFromJsonAsync<List<string>>(ct) ?? new List<string>();
    }

    public async Task<List<string>> GetNamesAutocompleteAsync(string query, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"api/vaccines/names-autocomplete?query={query}", ct);
        return await response.Content.ReadFromJsonAsync<List<string>>(ct) ?? new List<string>();
    }
}
