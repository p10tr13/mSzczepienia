using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces
{
    public interface IVaccinationRecordRepository
    {
        Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordOverviewAsync(String firebaseUid, GetVaccinationRecordOverviewQuery query, CancellationToken ct);
        Task<List<VaccinationRecordDetailsDto>> GetVaccinationRecordsDetailsAsync(string firebaseUid, GetVaccinationRecordDetailsQuery query, CancellationToken ct);
        Task<PagedResult<VaccinationRecordSimpleListItemDto>> GetVaccinationRecordsByCitizenIdAsync(GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct);
        Task AddAsync(VaccinationRecord record, CancellationToken ct);
        Task<VaccinationRecord?> GetById(int id, CancellationToken ct);
        Task<VaccinationRecord?> GetByIdNoTrackingAsync(int id, CancellationToken ct);
        void Update(VaccinationRecord record);
        void Delete(VaccinationRecord record);
    }
}