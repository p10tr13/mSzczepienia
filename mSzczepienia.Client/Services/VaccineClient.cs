using mSzczepienia.Client.Services.Interfaces;
using System.Net.Http.Json;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Vaccines;
using Microsoft.AspNetCore.WebUtilities;

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

    public async Task<PagedResult<VaccineDto>> GetVaccinesAsync(string? Name, string? Manufacturer, string? Disease, int? Id, int Page, int PageSize, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["Name"] = Name,
            ["Manufacturer"] = Manufacturer,
            ["Disease"] = Disease,
            ["Id"] = Id?.ToString(),
            ["PageNumber"] = Page.ToString(),
            ["PageSize"] = PageSize.ToString()
        };

        var url = QueryHelpers.AddQueryString("api/vaccines/search", queryParams);

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>(cancellationToken: ct);
            return result ?? new PagedResult<VaccineDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
