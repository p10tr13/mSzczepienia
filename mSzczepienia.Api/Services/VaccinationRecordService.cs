using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;

namespace mSzczepienia.Api.Services;

public class VaccinationRecordService : IVaccinationRecordService
{
    private readonly IVaccinationRecordRepository _vaccinationRecordRepository;

    public VaccinationRecordService(IVaccinationRecordRepository vaccinationRecordRepository)
    {
        _vaccinationRecordRepository = vaccinationRecordRepository;
    }

    public Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsOverviewAsync(string firebaseUid, GetVaccinationRecordOverviewQuery query, CancellationToken ct)
    {
        return _vaccinationRecordRepository.GetVaccinationRecordOverviewAsync(firebaseUid, query, ct);
    }

    public Task<List<VaccinationRecordDetailsDto>> GetVaccinationRecordsDetailsAsync(string firebaseUid, GetVaccinationRecordDetailsQuery query, CancellationToken ct)
    {
        return _vaccinationRecordRepository.GetVaccinationRecordsDetailsAsync(firebaseUid, query, ct);
    }
    
}