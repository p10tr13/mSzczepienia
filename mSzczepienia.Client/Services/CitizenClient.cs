using Microsoft.AspNetCore.WebUtilities;
using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using System.Net.Http.Json;

namespace mSzczepienia.Client.Services;

public class CitizenClient : ICitizenClient
{
    private readonly HttpClient _httpClient;

    private string citizenControllerUrl = "api/citizens";

    public CitizenClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<SimpleCitizenDto>> GetCitizensAsync(GetCitizensQuery query, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "Name", query.Name },
            { "Surname", query.Surname },
            { "Pesel", query.Pesel },
            { "SearchInWorkersFacilityOnly", query.SearchInWorkersFacilityOnly.ToString() },
            { "PageNumber", query.PageNumber.ToString() },
            { "PageSize", query.PageSize.ToString() }
        };

        var url = QueryHelpers.AddQueryString($"{citizenControllerUrl}/search", queryParams);

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<SimpleCitizenDto>>(cancellationToken: ct);
            return result ?? new PagedResult<SimpleCitizenDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
