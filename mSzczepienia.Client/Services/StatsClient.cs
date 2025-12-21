using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO.Stats;
using System.Net.Http.Json;

namespace mSzczepienia.Client.Services;

public class StatsClient : IStatsClient
{
    private readonly HttpClient _httpClient;

    public StatsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardStatsDto> GetAdminStatsAsync(CancellationToken ct)
    {
        return await _httpClient.GetFromJsonAsync<DashboardStatsDto>("api/stats/admin-dashboard-stats", ct)
               ?? new DashboardStatsDto();
    }

    public async Task<DashboardStatsDto> GetFacilityStatsAsync(int facilityId, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"api/stats/facility/{facilityId}", ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<DashboardStatsDto>(cancellationToken: ct);
            return result ?? new DashboardStatsDto();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
