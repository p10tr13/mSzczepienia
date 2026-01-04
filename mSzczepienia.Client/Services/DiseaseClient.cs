using mSzczepienia.Client.Services.Interfaces;
using System.Net.Http.Json;
using mSzczepienia.Shared.DTO.Diseases;

namespace mSzczepienia.Client.Services;

public class DiseaseClient : IDiseaseClient
{
    private readonly HttpClient _httpClient;

    private string diseaseControllerUrl = "api/diseases";

    public DiseaseClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DiseaseDto>> GetDiseasesAutocompleteAsync(string query, int count, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{diseaseControllerUrl}/names-autocomplete?query={query}&count={count}", ct);

        if (response.IsSuccessStatusCode)
        {
            var diseases = await response.Content.ReadFromJsonAsync<List<DiseaseDto>>(cancellationToken: ct);
            return diseases ?? new List<DiseaseDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
