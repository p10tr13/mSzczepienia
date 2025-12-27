using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IVaccinationRecordClient
{
    Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsByCitizenIdAsync(GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct);
    Task AddVaccinationRecordAsync(CreateVaccinationRecordDto dto, CancellationToken ct);
}