using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Shared.Models;

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

    public async Task<PagedResult<VaccinationRecordSimpleListItemDto>> GetVaccinationRecordsByCitizenIdAsync(string requesterFirebaseUid, GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct)
    {
        _logger.LogInformation("Vaccination records for citizen {CitizenId} requested by {RequesterUid}", query.CitizenId, requesterFirebaseUid);
        return await _vaccinationRecordRepository.GetVaccinationRecordsByCitizenIdAsync(query, ct);
    }

    public async Task<VaccinationRecordEditDto?> GetVaccinationRecordByIdAsync(string requesterFirebaseUid, int recordId, CancellationToken ct)
    {
        _logger.LogInformation("User {RequesterUid} requests vaccination record {RecordId}", requesterFirebaseUid, recordId);
        return await _vaccinationRecordRepository.GetVaccinationRecordByIdAsync(recordId, ct);
    }

    public async Task UpdateVaccinationRecordAsync(string requesterFirebaseUid, int recordId, UpdateVaccinationRecordDto dto, CancellationToken ct)
    {
        _logger.LogInformation("User {RequesterUid} updates vaccination record {RecordId}: date {VaccinationDate}, dose {Dose}, batch {Batch}", requesterFirebaseUid, recordId, dto.vaccinationDate, dto.doseNumber, dto.batch);
        await _vaccinationRecordRepository.UpdateVaccinationRecordAsync(recordId, dto, ct);
    }

    public async Task DeleteVaccinationRecordAsync(string requesterFirebaseUid, int recordId, CancellationToken ct)
    {
        _logger.LogInformation("User {RequesterUid} deletes vaccination record {RecordId}", requesterFirebaseUid, recordId);
        await _vaccinationRecordRepository.DeleteVaccinationRecordAsync(recordId, ct);
    }

    public async Task AddVaccinationRecordAsync(string requesterFirebaseUid, CreateVaccinationRecordDto dto, CancellationToken ct)
    {
        _logger.LogInformation("User {RequesterUid} adds vaccination record for citizen {CitizenId}: vaccine {VaccineId}, date {VaccinationDate}, dose {DoseNumber}, batch {Batch}",
            requesterFirebaseUid, dto.citizenId, dto.vaccineId, dto.vaccinationDate, dto.doseNumber, dto.batch);

        await _vaccinationRecordRepository.AddVaccinationRecordAsync(dto, ct);
    }

    public async Task CreateRecordsFromEntriesAsync(int citizenId, List<VaccinationEntry> validEntries, CancellationToken ct)
    {
        foreach (var entry in validEntries)
        {
            _logger.LogInformation("Creating vaccination record for citizen {CitizenId} from OCR entry {EntryId}: vaccine {VaccineId}, date {Date}, batch {Batch}",
                citizenId, entry.vaccinationEntryId, entry.vaccineId, entry.date, entry.batch);
            var newRecord = new VaccinationRecord
            {
                citizenId = citizenId,
                vaccineId = entry.vaccineId ?? throw new Exception("Vaccine is required"),
                vaccinationDate = entry.date ?? throw new Exception("Vaccination date is required"),
                batch = entry.batch ?? string.Empty,
                doseNumber = "unknown",
                sourceEntry = entry,
                creationDate = DateTime.UtcNow
            };
            await _vaccinationRecordRepository.AddAsync(newRecord, ct);
        }
    }
}