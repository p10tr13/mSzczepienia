using Microsoft.AspNetCore.WebUtilities;
using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO;
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
}
