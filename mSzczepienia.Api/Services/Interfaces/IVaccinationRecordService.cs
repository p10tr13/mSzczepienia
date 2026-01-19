using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IVaccinationRecordService
{
    Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsOverviewAsync(string firebaseUid, GetVaccinationRecordOverviewQuery query, CancellationToken ct);
    Task<List<VaccinationRecordDetailsDto>> GetVaccinationRecordsDetailsAsync(string firebaseUid, GetVaccinationRecordDetailsQuery query, CancellationToken ct);
    Task<PagedResult<VaccinationRecordSimpleListItemDto>> GetVaccinationRecordsByCitizenIdAsync(string requesterFirebaseUid, GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct);
    Task AddVaccinationRecordAsync(string requesterFirebaseUid, CreateVaccinationRecordDto dto, CancellationToken ct);
    Task<VaccinationRecordEditDto?> GetVaccinationRecordByIdAsync(string requesterFirebaseUid, int recordId, CancellationToken ct);
    Task UpdateVaccinationRecordAsync(string requesterFirebaseUid, int recordId, UpdateVaccinationRecordDto dto, CancellationToken ct);
    Task DeleteVaccinationRecordAsync(string requesterFirebaseUid, int recordId, CancellationToken ct);
    Task CreateRecordsFromEntriesAsync(int citizenId, List<VaccinationEntry> validEntries, CancellationToken ct);
    Task RemindAboutNextVaccinationsAsync(CancellationToken ct);
}
