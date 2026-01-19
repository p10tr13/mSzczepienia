using Microsoft.AspNetCore.WebUtilities;
using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Shared.DTO.Queries;
using System.Net.Http.Json;

namespace mSzczepienia.Client.Services;

public class PersonnelClient : IPersonnelClient
{
    private readonly HttpClient _httpClient;

    private string personnelControllerUrl = "api/personnel";

    public PersonnelClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<PersonnelDto>> GetPersonnelByFacilityAsync(int facilityId, int pageIndex, int pageSize, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "FacilityId", facilityId.ToString() },
            { "PageNumber", pageIndex.ToString() },
            { "PageSize", pageSize.ToString() }
        };

        var url = QueryHelpers.AddQueryString($"{personnelControllerUrl}", queryParams);

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>(cancellationToken: ct);
            return result ?? new PagedResult<PersonnelDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<PagedResult<PersonnelDto>> SearchPersonnelAsync(SearchPersonnelQuery query, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            {"Page", query.Page.ToString() },
            {"PageSize", query.PageSize.ToString() },
            {"FirstName", query.FirstName },
            {"Surname", query.Surname },
            {"Email", query.Email },
            {"FacilityName", query.FacilityName },
            {"FacilityCity", query.FacilityCity },
            {"FacilityStreet", query.FacilityStreet },
            {"FacilityPostalCode", query.FacilityPostalCode },
        };

        var url = QueryHelpers.AddQueryString($"{personnelControllerUrl}/search", queryParams);
        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<PersonnelDto>>(cancellationToken: ct);
            return result ?? new PagedResult<PersonnelDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<DetailedPersonnelDto?> GetPersonnelByIdAsync(int personnelId, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{personnelControllerUrl}/{personnelId}", ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<DetailedPersonnelDto>(cancellationToken: ct);
            return result ?? new DetailedPersonnelDto();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task UpdatePersonnelAsync(int personnelId, UpdatePersonnelDto updatePersonnelDto, CancellationToken ct)
    {
        var response = await _httpClient.PutAsJsonAsync($"{personnelControllerUrl}/{personnelId}", updatePersonnelDto, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
