using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO.Accounts;
using System.Net.Http.Json;

namespace mSzczepienia.Client.Services;

public class AccountClient : IAccountClient
{
    private readonly HttpClient _httpClient;
    private const string accountUrl = "api/account";

    public AccountClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProfileDto?> GetProfileAsync(CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{accountUrl}/profile", ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ProfileDto>(cancellationToken: ct);
            return result;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return null;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
