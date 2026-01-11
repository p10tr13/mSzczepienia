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
    private readonly ICitizenRepository _citizenRepository;
    private readonly IVaccineRepository _vaccineRepository;
    private readonly ICitizenDeviceTokenRepository _citizenDeviceTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VaccinationRecordService> _logger;
    private readonly INotificationService _notificationService;

    public VaccinationRecordService(IVaccinationRecordRepository vaccinationRecordRepository, ICitizenRepository citizenRepository, IVaccineRepository vaccineRepository,
        ILogger<VaccinationRecordService> logger, IUnitOfWork unitOfWork, ICitizenDeviceTokenRepository citizenDeviceTokenRepository, INotificationService notificationService)
    {
        _vaccinationRecordRepository = vaccinationRecordRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _citizenRepository = citizenRepository;
        _citizenDeviceTokenRepository = citizenDeviceTokenRepository;
        _vaccineRepository = vaccineRepository;
        _notificationService = notificationService;
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
        var record = await _vaccinationRecordRepository.GetById(recordId, ct);
        if (record == null)
        {
            _logger.LogWarning("Vaccination record {RecordId} not found for user {RequesterUid}", recordId, requesterFirebaseUid);
            return null;
        }
        
        var vaccine = await _vaccineRepository.GetVaccineDefinitionByIdNoTrackingAsync(record.vaccineId, ct);
        if (vaccine == null)
        {
            _logger.LogWarning("Vaccine {VaccineId} not found for vaccination record {RecordId} requested by user {RequesterUid}", record.vaccineId, recordId, requesterFirebaseUid);
            return null;
        }

        var citizen = await _citizenRepository.GetByIdNoTrackingAsync(record.citizenId, ct);
        if (citizen == null)
        {
            _logger.LogWarning("Citizen {CitizenId} not found for vaccination record {RecordId} requested by user {RequesterUid}", record.citizenId, recordId, requesterFirebaseUid);
            return null;
        }

        return new VaccinationRecordEditDto
        {
            recordId = record.recordId,
            citizenId = citizen.citizenId,
            name = citizen.account.name,
            surname = citizen.account.surname,
            pesel = citizen.pesel,
            dateOfBirth = citizen.account.dateOfBirth,
            vaccineId = vaccine.vaccineId,
            vaccineName = vaccine.vaccineName,
            manufacturer = vaccine.manufacturer,
            vaccinationDate = record.vaccinationDate,
            nextVaccinationDate = record.nextVaccinationDate,
            doseNumber = record.doseNumber,
            batch = record.batch,
            remarks = record.remarks
        };
    }

    public async Task UpdateVaccinationRecordAsync(string requesterFirebaseUid, int recordId, UpdateVaccinationRecordDto dto, CancellationToken ct)
    {
        _logger.LogInformation("User {RequesterUid} updates vaccination record {RecordId}: date {VaccinationDate}, dose {Dose}, batch {Batch}", requesterFirebaseUid, recordId, dto.vaccinationDate, dto.doseNumber, dto.batch);

        if (dto.vaccinationDate == null)
            throw new ArgumentNullException("Vaccination date cannot be null.");

        var record = await _vaccinationRecordRepository.GetById(recordId, ct);
        if (record == null)
        {
            _logger.LogWarning("Vaccination record {RecordId} not found for update by user {RequesterUid}", recordId, requesterFirebaseUid);
            throw new InvalidOperationException("Vaccination record not found.");
        }

        record.vaccinationDate = (DateTime)dto.vaccinationDate;
        record.nextVaccinationDate = dto.nextVaccinationDate;
        record.doseNumber = dto.doseNumber;
        record.batch = dto.batch;
        record.remarks = dto.remarks;

        _vaccinationRecordRepository.Update(record);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteVaccinationRecordAsync(string requesterFirebaseUid, int recordId, CancellationToken ct)
    {
        _logger.LogInformation("User {RequesterUid} deletes vaccination record {RecordId}", requesterFirebaseUid, recordId);

        var record = await _vaccinationRecordRepository.GetById(recordId, ct);
        if (record == null)
        {
            _logger.LogWarning("Vaccination record {RecordId} not found for deletion by user {RequesterUid}", recordId, requesterFirebaseUid);
            throw new InvalidOperationException("Vaccination record not found.");
        }

        _vaccinationRecordRepository.Delete(record);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task AddVaccinationRecordAsync(string requesterFirebaseUid, CreateVaccinationRecordDto dto, CancellationToken ct)
    {
        _logger.LogInformation("User {RequesterUid} adds vaccination record for citizen {CitizenId}: vaccine {VaccineId}, date {VaccinationDate}, dose {DoseNumber}, batch {Batch}",
            requesterFirebaseUid, dto.citizenId, dto.vaccineId, dto.vaccinationDate, dto.doseNumber, dto.batch);

        var citizen = await _citizenRepository.GetByIdNoTrackingAsync(dto.citizenId, ct);
        if (citizen == null)
        {
            _logger.LogWarning("Citizen {CitizenId} not found when user {RequesterUid} tried to add vaccination record", dto.citizenId, requesterFirebaseUid);
            throw new InvalidOperationException("Citizen not found.");
        }

        var vaccine = await _vaccineRepository.GetVaccineDefinitionByIdNoTrackingAsync(dto.vaccineId, ct);
        if (vaccine == null)
        {
            _logger.LogWarning("Vaccine {VaccineId} not found when user {RequesterUid} tried to add vaccination record", dto.vaccineId, requesterFirebaseUid);
            throw new InvalidOperationException("Vaccine definition not found.");
        }

        var record = new VaccinationRecord
        {
            citizenId = dto.citizenId,
            vaccineId = dto.vaccineId,
            vaccinationDate = dto.vaccinationDate,
            doseNumber = dto.doseNumber,
            nextVaccinationDate = dto.nextVaccinationDate,
            batch = dto.batch,
            sourceEntryId = dto.sourceEntryId,
            remarks = dto.remarks,
            creationDate = DateTime.UtcNow
        };

        await _vaccinationRecordRepository.AddAsync(record, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        try
        {
            var tokens = await _citizenDeviceTokenRepository.GetTokensByCitizenIdAsync(dto.citizenId, ct);

            foreach (var token in tokens)
            {
                string title;
                string subtitle;
                if (token.language == "pl")
                {
                    title = "Dodano nowe szczepienie";
                    subtitle = $"Dodano szczepienie - {record.vaccineDefinition.vaccineName}";
                }
                else
                {
                    title = "New vaccination added";
                    subtitle = $"Added vaccination - {record.vaccineDefinition.vaccineName}";
                }
                await _notificationService.SendNotificationAsync(token.deviceToken, title, subtitle);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification for new vaccination record for citizen {CitizenId}", dto.citizenId);
        }
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

    public async Task RemindAboutNextVaccinationsAsync(CancellationToken ct)
    {
        var tokens = await _citizenDeviceTokenRepository.GetDeviceTokensForNextVaccinationsAsync(ct);

        foreach (var token in tokens)
        {
            string title;
            string subtitle;
            if (token.language == "pl")
            {
                title = $"Nadchodzi termin kolejnego szczepienia: {token.vaccineName}";
                subtitle = "Przypominamy o nadchodz¹cym terminie szczepienia za 7 dni.";
            }
            else
            {
                title = $"Upcoming vaccination appointment: {token.vaccineName}";
                subtitle = "This is a reminder for your upcoming vaccination in 7 days.";
            }
            try
            {
                await _notificationService.SendNotificationAsync(token.deviceToken, title, subtitle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send vaccination reminder notification");
            }
        }

    }
}