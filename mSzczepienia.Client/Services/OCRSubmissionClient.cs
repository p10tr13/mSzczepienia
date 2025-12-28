using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace mSzczepienia.Client.Services;

public class OCRSubmissionClient : IOCRSubmissionClient
{
    private readonly HttpClient _httpClient;

    private string ocrSubmissionControlerUrl = "api/ocrsubmission";

    public OCRSubmissionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<SearchOCRSubmissionDTO>> GetDashboardOCRSubmissions(int PageSize, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            {"Page", "1" },
            {"PageSize", PageSize.ToString() }
        };

        var url = QueryHelpers.AddQueryString($"{ocrSubmissionControlerUrl}/pending/worker", queryParams);

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>(cancellationToken: ct);
            return result ?? new PagedResult<SearchOCRSubmissionDTO>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task<PagedResult<SearchOCRSubmissionDTO>> GetOCRSubmissionsAsync(SearchOCRSubmissionQueryDto query, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            {"Page", query.Page.ToString() },
            {"PageSize", query.PageSize.ToString() },
            {"FirstName", query.FirstName },
            {"Surname", query.Surname },
            {"Pesel", query.Pesel },
            {"SearchInWorkersFacilityOnly", query.SearchInWorkersFacilityOnly.ToString() }
        };

        var url = QueryHelpers.AddQueryString($"{ocrSubmissionControlerUrl}/pending/worker", queryParams);

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<SearchOCRSubmissionDTO>>(cancellationToken: ct);
            return result ?? new PagedResult<SearchOCRSubmissionDTO>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
