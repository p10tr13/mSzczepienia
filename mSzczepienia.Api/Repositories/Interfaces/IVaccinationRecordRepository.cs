using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.VaccinationRecords;

namespace mSzczepienia.Api.Repositories.Interfaces
{
    public interface IVaccinationRecordRepository
    {
        Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordOverviewAsync(String firebaseUid, GetVaccinationRecordOverviewQuery query, CancellationToken ct);
        Task<List<VaccinationRecordDetailsDto>> GetVaccinationRecordsDetailsAsync(string firebaseUid, GetVaccinationRecordDetailsQuery query, CancellationToken ct);
        Task<PagedResult<VaccinationRecordSimpleListItemDto>> GetVaccinationRecordsByCitizenIdAsync(GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct);
        Task AddVaccinationRecordAsync(CreateVaccinationRecordDto dto, CancellationToken ct);
        Task<VaccinationRecordEditDto?> GetVaccinationRecordByIdAsync(int recordId, CancellationToken ct);
        Task UpdateVaccinationRecordAsync(int recordId, UpdateVaccinationRecordDto dto, CancellationToken ct);
        Task DeleteVaccinationRecordAsync(int recordId, CancellationToken ct);
    }
}