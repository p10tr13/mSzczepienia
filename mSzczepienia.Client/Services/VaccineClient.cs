using mSzczepienia.Client.Services.Interfaces;
using System.Net.Http.Json;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Vaccines;
using Microsoft.AspNetCore.WebUtilities;

namespace mSzczepienia.Client.Services;

public class VaccineClient : IVaccineClient
{
    private readonly HttpClient _httpClient;
    private readonly string _controllerUrl = "api/vaccines";

    public VaccineClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetManufacturersAutocompleteAsync(string query, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{_controllerUrl}/manufacturers-autocomplete?query={query}", ct);
        return await response.Content.ReadFromJsonAsync<List<string>>(ct) ?? new List<string>();
    }

    public async Task<List<string>> GetNamesAutocompleteAsync(string query, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{_controllerUrl}/names-autocomplete?query={query}", ct);
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

        var url = QueryHelpers.AddQueryString($"{_controllerUrl}/search", queryParams);

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

    public async Task<VaccineDto?> GetVaccineByIdAsync(int id, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{_controllerUrl}/{id}", ct);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<VaccineDto>(cancellationToken: ct);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<List<VaccineDto>> SearchVaccinesAsync(string term, int maxResults, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "SearchPhrase", term },
            { "PageSize", maxResults.ToString() },
            { "PageNumber", 1.ToString() }
        };

        var url = QueryHelpers.AddQueryString($"{_controllerUrl}/search", queryParams);
        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccineDto>>(cancellationToken: ct);
            return result?.Items ?? new List<VaccineDto>();
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken: ct);
        var message = string.IsNullOrWhiteSpace(content) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : content;
        throw new HttpRequestException(message, null, response.StatusCode);
    }

    public async Task CreateVaccineAsync(CreateVaccineDto dto, CancellationToken ct)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_controllerUrl}/create", dto, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(content) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : content;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<DetailedVaccineDto?> GetDetailedVaccineByIdAsync(int id, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{_controllerUrl}/detailed/{id}", ct);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<DetailedVaccineDto>(cancellationToken: ct);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task UpdateVaccineAsync(int id, CreateVaccineDto dto, CancellationToken ct)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_controllerUrl}/{id}", dto, ct);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(content) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : content;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
