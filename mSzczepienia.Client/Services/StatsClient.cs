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
}
