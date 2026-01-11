using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;
using System.Security.Claims;

namespace mSzczepienia.Api.Services;

public class OCRSubmissionsService : IOCRSubmissionsService
{
    private readonly IOCRSubmissionRepository _ocrSubmissionRepository;
    private readonly IPersonnelRepository _personnelRepository;
    private readonly ICitizenRepository _citizenRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IBlobService _blobService;
    private readonly ICitizenDeviceTokenRepository _citizenDeviceTokenRepository;
    private readonly IVaccineRepository _vaccineRepository;
    private readonly INotificationService _notificationService;
    private readonly IVaccinationEntryRepository _vaccinationEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVaccinationRecordService _vaccinationRecordService;
    private readonly IAzureOcrService _azureOcrService;
    private readonly IUtilitiesService _utilitiesService;

    public OCRSubmissionsService(IOCRSubmissionRepository ocrSubmissionRepository, IPersonnelRepository personnelRepository, ICitizenRepository citizenRepository, IFacilityRepository facilityRepository, IBlobService blobService,
        ICitizenDeviceTokenRepository citizenDeviceTokenRepository, IVaccineRepository vaccineRepository, INotificationService notificationService, IVaccinationEntryRepository vaccinationEntryRepository, IUnitOfWork unitOfWork,
        IVaccinationRecordService vaccinationRecordService, IAzureOcrService azureOcrService, IUtilitiesService utilitiesService)
    {
        _ocrSubmissionRepository = ocrSubmissionRepository;
        _personnelRepository = personnelRepository;
        _citizenRepository = citizenRepository;
        _facilityRepository = facilityRepository;
        _blobService = blobService;
        _citizenDeviceTokenRepository = citizenDeviceTokenRepository;
        _vaccineRepository = vaccineRepository;
        _notificationService = notificationService;
        _vaccinationEntryRepository = vaccinationEntryRepository;
        _unitOfWork = unitOfWork;
        _vaccinationRecordService = vaccinationRecordService;
        _azureOcrService = azureOcrService;
        _utilitiesService = utilitiesService;
    }

    public async Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingForWorkerAsync(ClaimsPrincipal user, SearchOCRSubmissionQueryDto query, CancellationToken ct)
    {
        string? firebaseUid = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            firebaseUid = user.FindFirst("user_id")?.Value ?? user.FindFirst("uid")?.Value;
        }

        if (string.IsNullOrEmpty(firebaseUid))
        {
            return new PagedResult<SearchOCRSubmissionDTO>();
        }

        // If worker requested all submissions (not limited to their facility), return global pending list
        if (query != null && query.SearchInWorkersFacilityOnly == false)
        {
            return await _ocrSubmissionRepository.GetPendingAsync(query, ct);
        }

        var facilityId = await _personnelRepository.GetFacilityIdForWorkerByFirebaseUidAsync(firebaseUid, ct);
        if (!facilityId.HasValue)
        {
            return new PagedResult<SearchOCRSubmissionDTO>();
        }

        return await _ocrSubmissionRepository.GetPendingForFacilityAsync(facilityId.Value, query ?? new SearchOCRSubmissionQueryDto(), ct);
    }

    public async Task<List<VaccinationEntryDto>> ProcessOcrSubmissionAsync(string firebaseUid, List<IFormFile> files, CancellationToken ct)
    {
        string uniqueGuid = Guid.NewGuid().ToString();
        string directoryUrl = $"{firebaseUid}/{uniqueGuid}/";

        // UPLOAD FILES TO BLOB STORAGE
        await _blobService.UploadMultipleFilesAsync(directoryUrl, files);

        // ADD NEW OCR SUBMISSION
        await _ocrSubmissionRepository.AddOcrSubmissionAsync(firebaseUid, directoryUrl, files, ct);

        // OCR PROCESS
        var ocrEntries = await _azureOcrService.ProcessFilesAsync(files, ct);
        var entriesDto = new List<VaccinationEntryDto>();

        var vaccineNames = await _vaccineRepository.GetVaccineNamesAsync(ct);

        int id = 0;
        foreach (var entry in ocrEntries)
        {
            // Use levenstein distance to find best matching vaccine name
            // If string.Empty - match not found
            string bestMatchVaccineName = _utilitiesService.ChooseBestMatch(entry.vaccineName, vaccineNames);

            // Parse date
            DateTime dateTime = _utilitiesService.ParseDateTime(entry.vaccinationDate);

            // Parse batch
            string batch = string.Empty;
            if (entry.batch != null)
            {
                batch = entry.batch.Trim().ToUpper();
            }

            if (bestMatchVaccineName != string.Empty)
            {
                entriesDto.Add(new VaccinationEntryDto
                {
                    Id = id,
                    VaccineNameOcr = entry.vaccineName,
                    DateOcr = entry.vaccinationDate,
                    BatchOcr = entry.batch,
                    VaccineName = bestMatchVaccineName,
                    Date = dateTime,
                    Batch = batch,
                    Status = VaccinationEntryStatus.WaitingForReview.ToString()
                });
                id++;
            }
        }

        return entriesDto;
    }

    public async Task<OCRSubmissionPhotoReviewDto?> GetSubmissionPhotoReviewAsync(int submissionId, CancellationToken ct)
    {
        var submission = await _ocrSubmissionRepository.GetByIdAsync(submissionId, ct);
        if (submission == null)
            return null;

        var citizenDetails = await _citizenRepository.GetCitizenDetailsByIdAsync(submission.citizenId, ct);

        int? facilityId = await _citizenRepository.GetFacilityIdByCitizenIdAsync(submission.citizenId, ct);
        MedicalFacility? facility = null;
        if (facilityId.HasValue)
        {
            facility = await _facilityRepository.GetByIdAsNoTrackingAsync(facilityId.Value, ct);
        }

        List<string> photos = new List<string>();
        try
        {
            photos = await _blobService.GetSasUrlsForDirectoryAsync(submission.photoUrl);
        }
        catch
        {
            photos = new List<string>();
        }

        var dto = new OCRSubmissionPhotoReviewDto
        {
            SubmissionId = submission.submissionId,
            SubmissionDate = submission.submissionDate,
            PatientFirstName = citizenDetails?.name ?? string.Empty,
            PatientSurame = citizenDetails?.surname ?? string.Empty,
            Pesel = citizenDetails?.pesel ?? string.Empty,
            PatientDateOfBirth = citizenDetails?.dateOfBirth ?? DateTime.MinValue,
            Photos = photos,
            MedicalFacilityName = facility?.facilityName ?? citizenDetails?.facilityName ?? string.Empty,
            MedicalFacilityStreet = facility?.street ?? string.Empty,
            MedicalFacilityCity = facility?.city ?? string.Empty,
            MedicalFacilityPostalCode = facility?.postalCode ?? string.Empty
        };

        return dto;
    }

    public async Task<bool> RejectSubmissionAsync(string firebaseUid, int submissionId, CancellationToken ct)
    {
        var submission = await _ocrSubmissionRepository.GetByIdAsync(submissionId, ct);
        if (submission == null)
            return false;

        var citizen = await _citizenRepository.GetByIdAsync(submission.citizenId, ct);
        if (citizen == null)
            throw new InvalidOperationException("Citizen not found.");

        var personnel = await _personnelRepository.GetByFirebaseUidAsync(firebaseUid, ct);
        if (personnel == null)
            throw new InvalidOperationException("Personnel not found.");

        var entries = await _vaccinationEntryRepository.GetBySubmissionIdAsync(submissionId, ct);

        if (submission.status != OcrSubmissionStatus.Pending)
        {
            throw new InvalidOperationException("Only pending submissions can be rejected.");
        }

        submission.status = OcrSubmissionStatus.Rejected;
        submission.verifierId = personnel.personnelId;
        submission.verificationDate = DateTime.UtcNow;
        _ocrSubmissionRepository.Update(submission);

        citizen.submissionStatus = SubmissionStatus.Rejected;
        _citizenRepository.Update(citizen);

        _vaccinationEntryRepository.DeleteRange(entries);
        await _unitOfWork.SaveChangesAsync(ct);

        // After successful rejection, fetch device tokens and send notifications
        try
        {
            var tokens = await _citizenDeviceTokenRepository.GetTokensByCitizenIdAsync(submission.citizenId, ct);
            foreach (var t in tokens)
            {
                var (deviceToken, language) = t;
                string title, subtitle;
                if (language == "pl")
                {
                    title = "Odrzucono Twoje zgłoszenie";
                    subtitle = $"Odrzucono Twoje zgłoszenie o numerze {submission.submissionId}";
                }
                else
                {
                    title = "Your submission has been rejected";
                    subtitle = $"Your submission no. {submission.submissionId} has been rejected";
                }

                await _notificationService.SendNotificationAsync(deviceToken, title, subtitle);
            }
        }
        catch
        {
            // swallow notification errors - rejection already persisted
        }

        return true;
    }

    public async Task<OCRSubmissionEntriesReviewDto?> GetSubmissionEntriesReviewAsync(int submissionId, CancellationToken ct)
    {
        var submission = await _ocrSubmissionRepository.GetByIdAsync(submissionId, ct);
        if (submission == null)
            return null;

        List<string> photos;
        try
        {
            photos = await _blobService.GetSasUrlsForDirectoryAsync(submission.photoUrl);
        }
        catch
        {
            photos = new List<string>();
        }

        var entries = await _vaccinationEntryRepository.GetBySubmissionIdAsync(submissionId, ct);

        var vaccineIds = entries.Where(e => e.vaccineId != null).Select(e => e.vaccineId!.Value).Distinct().ToList();

        var vaccines = await _vaccineRepository.GetVaccinesByIdsAsync(vaccineIds, ct);
        var vaccinesDict = vaccines.ToDictionary(v => v.VaccineId);

        var dto = new OCRSubmissionEntriesReviewDto
        {
            SubmissionId = submission.submissionId,
            Photos = photos,
            VaccinationEntries = entries.Select(e =>
            {
                vaccinesDict.TryGetValue(e.vaccineId ?? 0, out var vaccine);

                return new DetailedVaccinationEntryDto
                {
                    Id = e.vaccinationEntryId,
                    VaccineNameOcr = e.vaccineNameOcr,
                    DateOcr = e.dateOcr,
                    BatchOcr = e.batchOcr,
                    Vaccine = vaccine,
                    Date = e.date,
                    Batch = e.batch,
                    Status = e.status
                };
            }).ToList()
        };

        return dto;
    }

    public async Task ProcessSubmissionReviewAsync(string firebaseUid, ProcessSubmissionReviewDto request, CancellationToken ct)
    {
        var submission = await _ocrSubmissionRepository.GetByIdAsync(request.SubmissionId, ct);
        if (submission == null)
            throw new InvalidOperationException("Submission not found.");

        var citizen = await _citizenRepository.GetByIdAsync(submission.citizenId, ct);
        if (citizen == null)
            throw new InvalidOperationException("Citizen not found.");

        var personnel = await _personnelRepository.GetByFirebaseUidAsync(firebaseUid, ct);
        if (personnel == null)
            throw new InvalidOperationException("Personnel not found.");

        var entries = await _vaccinationEntryRepository.GetBySubmissionIdAsync(request.SubmissionId, ct);

        var entriesToPromote = new List<VaccinationEntry>();

        foreach (var entryDto in request.Entries)
        {
            VaccinationEntry entryToProcess = null;

            if (entryDto.Id == 0)
            {
                entryToProcess = new VaccinationEntry
                {
                    ocrSubmissionId = request.SubmissionId,
                    status = VaccinationEntryStatus.CreatedByUser,
                    vaccineNameOcr = "", dateOcr = "", batchOcr = "",
                    vaccineId = entryDto.Vaccine?.VaccineId,
                    batch = entryDto.Batch,
                    date = entryDto.Date
                };
                await _vaccinationEntryRepository.AddAsync(entryToProcess, ct);
            }
            else
            {
                entryToProcess = entries.FirstOrDefault(e => e.vaccinationEntryId == entryDto.Id);
                if (entryToProcess != null)
                {
                    entryToProcess.vaccineId = entryDto.Vaccine?.VaccineId;
                    entryToProcess.batch = entryDto.Batch;
                    entryToProcess.date = entryDto.Date;
                    entryToProcess.status = VaccinationEntryStatus.Processed;
                    _vaccinationEntryRepository.Update(entryToProcess);
                }
            }

            if (entryToProcess != null)
            {
                entriesToPromote.Add(entryToProcess);
            }
        }

        var incomingIds = request.Entries.Where(e => e.Id != 0).Select(e => e.Id).ToList();
        var entriesToDelete = entries.Where(e => !incomingIds.Contains(e.vaccinationEntryId));
        foreach (var del in entriesToDelete)
        {
            del.status = VaccinationEntryStatus.Deleted;
            _vaccinationEntryRepository.Update(del);
        }

        await _vaccinationRecordService.CreateRecordsFromEntriesAsync(submission.citizenId, entriesToPromote, ct);

        submission.status = OcrSubmissionStatus.Accepted;
        submission.verifierId = personnel.personnelId;
        submission.verificationDate = DateTime.UtcNow;
        _ocrSubmissionRepository.Update(submission);

        citizen.submissionStatus = SubmissionStatus.Accepted;
        _citizenRepository.Update(citizen);

        await _unitOfWork.SaveChangesAsync(ct);

        // After successful review, fetch device tokens and send notifications
        try
        {
            var tokens = await _citizenDeviceTokenRepository.GetTokensByCitizenIdAsync(submission.citizenId, ct);
            foreach (var t in tokens)
            {
                var (deviceToken, language) = t;
                string title, subtitle;
                if (language == "pl")
                {
                    title = "Twoje zgłoszenie zostało zaakceptowane";
                    subtitle = $"Twoje zgłoszenie o numerze {submission.submissionId} zostało zaakceptowane";
                }
                else
                {
                    title = "Your submission has been accepted";
                    subtitle = $"Your submission no. {submission.submissionId} has been accepted";
                }

                await _notificationService.SendNotificationAsync(deviceToken, title, subtitle);
            }
        }
        catch
        {
            // swallow notification errors - review already persisted
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteOcrSubmissionAsync(string firebaseUid, CancellationToken ct)
    {
        string directoryUrl = await _ocrSubmissionRepository.DeleteOcrSubmissionAsync(firebaseUid, ct);

        await _blobService.DeleteDirectoryAsync(directoryUrl);
    }

    public async Task UpdateOcrSubmissionWithEntries(string firebaseUid, List<VaccinationEntryDto> entries, CancellationToken ct)
    {
        await _ocrSubmissionRepository.UpdateOcrSubmissionWithEntries(firebaseUid, entries, ct);
    }
}
