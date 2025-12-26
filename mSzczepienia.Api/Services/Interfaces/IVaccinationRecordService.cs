using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IVaccinationRecordService
{
    Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsOverviewAsync(string firebaseUid, GetVaccinationRecordOverviewQuery query, CancellationToken ct);
    Task<List<VaccinationRecordDetailsDto>> GetVaccinationRecordsDetailsAsync(string firebaseUid, GetVaccinationRecordDetailsQuery query, CancellationToken ct);
    Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsByCitizenIdAsync(string requesterFirebaseUid, GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct);
}
