using Microsoft.AspNetCore.WebUtilities;
using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using System.Net.Http.Json;

namespace mSzczepienia.Client.Services;

public class FacilityClient : IFacilityClient
{
    private readonly HttpClient _httpClient;

    private string facilityControllerUrl = "api/facilities";

    public FacilityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetCitiesAutocompleteAsync(string query, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{facilityControllerUrl}/cities-autocomplete?query={query}", ct);
        var facilityNames = await response.Content.ReadFromJsonAsync<List<string>>(cancellationToken: ct);
        return facilityNames ?? new List<string>();
    }

    public async Task<PagedResult<SearchFacilityDto>> GetFacilitiesAsync(string? City, string? Street, string? PostalCode, int? Id, int Page, int PageSize,
        CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "City", City },
            { "Street", Street },
            { "ZipCode", PostalCode },
            { "Id", Id?.ToString() },
            { "PageNumber", Page.ToString() },
            { "PageSize", PageSize.ToString() }
        };

        var url = QueryHelpers.AddQueryString($"{facilityControllerUrl}/search", queryParams);

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>(cancellationToken: ct);
            return result ?? new PagedResult<SearchFacilityDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<PagedResult<SearchFacilityDto>> GetFacilitiesAsync(GetFacilitiesQuery query, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "City", query.City },
            { "Street", query.Street },
            { "ZipCode", query.ZipCode },
            { "Id", query.Id?.ToString() },
            { "SearchPhrase", query.SearchPhrase },
            { "PageNumber", query.PageNumber.ToString() },
            { "PageSize", query.PageSize.ToString() }
        };

        var url = QueryHelpers.AddQueryString($"{facilityControllerUrl}/search", queryParams);

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchFacilityDto>>(cancellationToken: ct);
            return result ?? new PagedResult<SearchFacilityDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<FacilityDto> GetFacilityByIdAsync(int id, CancellationToken ct)
    {
        var url = $"{facilityControllerUrl}/{id}";

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<FacilityDto>(cancellationToken: ct);
            return result ?? new FacilityDto();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
