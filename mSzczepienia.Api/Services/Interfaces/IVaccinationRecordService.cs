using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IVaccinationRecordService
{
    Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsOverviewAsync(String firebaseUid, GetVaccinationRecordOverviewQuery query, CancellationToken ct);
    Task<List<VaccinationRecordDetailsDto>> GetVaccinationRecordsDetailsAsync(string firebaseUid, GetVaccinationRecordDetailsQuery query, CancellationToken ct);
}
