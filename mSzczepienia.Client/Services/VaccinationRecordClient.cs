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

    public async Task<PagedResult<VaccinationRecordSimpleListItemDto>> GetVaccinationRecordsByCitizenIdAsync(GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct)
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
            var result = await response.Content.ReadFromJsonAsync<PagedResult<VaccinationRecordSimpleListItemDto>>(cancellationToken: ct);
            return result ?? new PagedResult<VaccinationRecordSimpleListItemDto>();
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

    public async Task<VaccinationRecordEditDto?> GetVaccinationRecordByIdAsync(int recordId, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{_controllerUrl}/{recordId}/edit", ct);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<VaccinationRecordEditDto>(cancellationToken: ct);
            return result;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(content) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : content;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task UpdateVaccinationRecordAsync(int recordId, UpdateVaccinationRecordDto dto, CancellationToken ct)
    {
        if (dto.vaccinationDate == null)
            throw new ArgumentNullException("Vaccination date cannot be null");
        dto.vaccinationDate = DateTime.SpecifyKind((DateTime)dto.vaccinationDate, DateTimeKind.Unspecified);
        if (dto.nextVaccinationDate != null)
            dto.nextVaccinationDate = DateTime.SpecifyKind((DateTime)dto.nextVaccinationDate, DateTimeKind.Unspecified);

        var response = await _httpClient.PutAsJsonAsync($"{_controllerUrl}/{recordId}", dto, ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(content) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : content;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }

    public async Task DeleteVaccinationRecordAsync(int recordId, CancellationToken ct)
    {
        var response = await _httpClient.DeleteAsync($"{_controllerUrl}/{recordId}", ct);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken: ct);
            var message = string.IsNullOrWhiteSpace(content) ? $"Server error: {response.ReasonPhrase} ({(int)response.StatusCode})" : content;
            throw new HttpRequestException(message, null, response.StatusCode);
        }
    }
}
