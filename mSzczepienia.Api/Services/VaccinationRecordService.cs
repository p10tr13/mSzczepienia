using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;

namespace mSzczepienia.Api.Services;

public class VaccinationRecordService : IVaccinationRecordService
{
    private readonly IVaccinationRecordRepository _vaccinationRecordRepository;
    private readonly ILogger<VaccinationRecordService> _logger;

    public VaccinationRecordService(IVaccinationRecordRepository vaccinationRecordRepository, ILogger<VaccinationRecordService> logger)
    {
        _vaccinationRecordRepository = vaccinationRecordRepository;
        _logger = logger;
    }

    public Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsOverviewAsync(string firebaseUid, GetVaccinationRecordOverviewQuery query, CancellationToken ct)
    {
        return _vaccinationRecordRepository.GetVaccinationRecordOverviewAsync(firebaseUid, query, ct);
    }

    public Task<List<VaccinationRecordDetailsDto>> GetVaccinationRecordsDetailsAsync(string firebaseUid, GetVaccinationRecordDetailsQuery query, CancellationToken ct)
    {
        return _vaccinationRecordRepository.GetVaccinationRecordsDetailsAsync(firebaseUid, query, ct);
    }

    public async Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordsByCitizenIdAsync(string requesterFirebaseUid, GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct)
    {
        _logger.LogInformation("Vaccination records for citizen {CitizenId} requested by {RequesterUid}", query.CitizenId, requesterFirebaseUid);
        return await _vaccinationRecordRepository.GetVaccinationRecordOverviewByCitizenIdAsync(query, ct);
    }
}