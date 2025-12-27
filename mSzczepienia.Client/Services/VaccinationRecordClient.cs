using Microsoft.AspNetCore.WebUtilities;
using mSzczepienia.Client.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using System.Net.Http.Json;

namespace mSzczepienia.Client.Services;

public class VaccinationRecordClient : IVaccinationRecordClient
{
    private readonly HttpClient _httpClient;
    private readonly string _controllerUrl = "api/vaccination-records";

    public VaccinationRecordClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsByCitizenIdAsync(GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct)
    {
        var queryParams = new Dictionary<string, string?>
        {
            { "CitizenId", query.CitizenId.ToString() },
            { "PageNumber", query.PageNumber.ToString() },
            { "PageSize", query.PageSize.ToString() },
            { "SortingField", query.SortingField },
            { "SortingDirection", query.SortingDirection }
        };

        var url = QueryHelpers.AddQueryString($"{_controllerUrl}/by-citizen", queryParams);

        var response = await _httpClient.GetAsync(url, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccinationRecordOverviewDto>>(cancellationToken: ct);
            return result ?? new PagedResult<VaccinationRecordOverviewDto>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(errorContent) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : errorContent;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task AddVaccinationRecordAsync(CreateVaccinationRecordDto dto, CancellationToken ct)
    {
        dto.vaccinationDate = DateTime.SpecifyKind(dto.vaccinationDate, DateTimeKind.Unspecified);
        if (dto.nextVaccinationDate != null)
            dto.nextVaccinationDate = DateTime.SpecifyKind((DateTime)dto.nextVaccinationDate, DateTimeKind.Unspecified);
        var response = await _httpClient.PostAsJsonAsync($"{_controllerUrl}/new", dto, ct);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(content) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : content;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
