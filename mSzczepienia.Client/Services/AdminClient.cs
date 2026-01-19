using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.DTO.Queries;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using mSzczepienia.Shared.DTO.Admins;

namespace mSzczepienia.Client.Services;

public class AdminClient : IAdminClient
{
    private readonly HttpClient _httpClient;

    private const string adminControllerUrl = "api/admin";

    public AdminClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<AccountDto>> SearchAdminsAsync(SearchAdminQuery query, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            {"Page", query.Page.ToString() },
            {"PageSize", query.PageSize.ToString() },
            {"FirstName", query.FirstName },
            {"Surname", query.Surname },
            {"Email", query.Email }
        };

        var url = QueryHelpers.AddQueryString($"{adminControllerUrl}/search", queryParams);
        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result =  await response.Content.ReadFromJsonAsync<PagedResult<AccountDto>>(cancellationToken: ct);
            return result ?? new PagedResult<AccountDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<DetailedAccountDto> GetAdminByIdAsync(Guid adminId, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{adminControllerUrl}/{adminId}", ct);

        if (response.IsSuccessStatusCode)
        {
            var result =  await response.Content.ReadFromJsonAsync<DetailedAccountDto>(cancellationToken: ct);
            return result ?? new DetailedAccountDto();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task UpdateAdminAsync(Guid adminId, UpdateAdminDto updateAdminDto, CancellationToken ct)
    {
        var response = await _httpClient.PutAsJsonAsync($"{adminControllerUrl}/{adminId}", updateAdminDto, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
