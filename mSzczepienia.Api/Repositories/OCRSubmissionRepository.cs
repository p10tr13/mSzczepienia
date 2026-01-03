using System.Data;
using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationEntries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories;

public class OCRSubmissionRepository : IOCRSubmissionRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IBlobService _blobService;
    public OCRSubmissionRepository(AppDbContext dbContext, IBlobService blobService)
    {
        _dbContext = dbContext;
        _blobService = blobService;
    }

    public async Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingForFacilityAsync(int facilityId, SearchOCRSubmissionQueryDto query, CancellationToken ct)
    {
        query = query ?? new SearchOCRSubmissionQueryDto();

        var submissionsQuery = _dbContext.ocrSubmissions.AsNoTracking().Include(s => s.citizen).ThenInclude(c => c.account)
            .Where(s => s.status == OcrSubmissionStatus.Pending && s.citizen.medicalFacility.facilityId == facilityId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.FirstName))
        {
            var name = query.FirstName.Trim();
            submissionsQuery = submissionsQuery.Where(s => s.citizen.account.name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(query.Surname))
        {
            var surname = query.Surname.Trim();
            submissionsQuery = submissionsQuery.Where(s => s.citizen.account.surname.Contains(surname));
        }

        if (!string.IsNullOrWhiteSpace(query.Pesel))
        {
            var pesel = query.Pesel.Trim();
            submissionsQuery = submissionsQuery.Where(s => s.citizen.pesel.Contains(pesel));
        }

        var totalCount = await submissionsQuery.CountAsync(ct);

        var items = await submissionsQuery
            .OrderBy(s => s.submissionDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new SearchOCRSubmissionDTO
            {
                ID = s.submissionId,
                Name = s.citizen.account.name,
                Surname = s.citizen.account.surname,
                SubmissionDate = s.submissionDate,
                Pesel = s.citizen.pesel
            })
            .ToListAsync(ct);

        return new PagedResult<SearchOCRSubmissionDTO>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<PagedResult<SearchOCRSubmissionDTO>> GetPendingAsync(SearchOCRSubmissionQueryDto query, CancellationToken ct)
        {
            query = query ?? new SearchOCRSubmissionQueryDto();

            if (query.SearchInWorkersFacilityOnly == true)
                throw new ArgumentException("Cannot search OCR submissions for specific facility with this mettod.");

            var submissionsQuery = _dbContext.ocrSubmissions.AsNoTracking().Include(s => s.citizen).ThenInclude(c => c.account)
                .Where(s => s.status == OcrSubmissionStatus.Pending).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FirstName))
            {
                var name = query.FirstName.Trim();
                submissionsQuery = submissionsQuery.Where(s => s.citizen.account.name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(query.Surname))
            {
                var surname = query.Surname.Trim();
                submissionsQuery = submissionsQuery.Where(s => s.citizen.account.surname.Contains(surname));
            }

            if (!string.IsNullOrWhiteSpace(query.Pesel))
            {
                var pesel = query.Pesel.Trim();
                submissionsQuery = submissionsQuery.Where(s => s.citizen.pesel.Contains(pesel));
            }

            var totalCount = await submissionsQuery.CountAsync(ct);

            var items = await submissionsQuery
                .OrderBy(s => s.submissionDate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(s => new SearchOCRSubmissionDTO
                {
                    ID = s.submissionId,
                    Name = s.citizen.account.name,
                    Surname = s.citizen.account.surname,
                    SubmissionDate = s.submissionDate,
                    Pesel = s.citizen.pesel
                })
                .ToListAsync(ct);

            return new PagedResult<SearchOCRSubmissionDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.Page,
                PageSize = query.PageSize
            };
        }

    public async Task<List<VaccinationEntryDto>> ProcessOcrSubmissionAsync(string firebaseUid, List<IFormFile> files, CancellationToken ct)
    {
        // ADD NEW OCR SUBMISSION RECORD
        var citizen = await _dbContext.citizens
            .AsNoTracking()
            .Include(c => c.account)
            .FirstOrDefaultAsync(c => c.account.firebaseUid == firebaseUid, ct);

        if (citizen == null)
        {
            throw new InvalidOperationException("Citizen not found for the provided Firebase UID.");
        }

        string uniqueGuid = Guid.NewGuid().ToString();
        string directoryUrl = $"{firebaseUid}/{uniqueGuid}/";

        var newSubmission = new OcrSubmission
        {
            citizenId = citizen.citizenId,
            submissionDate = DateTime.UtcNow,
            status = OcrSubmissionStatus.Uploaded,
            photoUrl = directoryUrl,
        };
        _dbContext.ocrSubmissions.Add(newSubmission);
        await _dbContext.SaveChangesAsync();


        // UPLOAD FILES TO BLOB STORAGE
        await _blobService.UploadMultipleFilesAsync(directoryUrl, files);


        // OCR PROCESS
        // PLACEHOLDER

        // RETURN PROCESSED ENTRIES
        List<VaccinationEntryDto> processedEntries = new List<VaccinationEntryDto>
        {
            new VaccinationEntryDto
            {
                vaccineName = "Vaccine A",
                vaccinationDate = DateTime.UtcNow.AddDays(-60)
            }
        };
        return processedEntries;
    }

    public async Task<OcrSubmission?> GetByIdAsync(int submissionId, CancellationToken ct)
    {
        return await _dbContext.ocrSubmissions
            .FirstOrDefaultAsync(s => s.submissionId == submissionId, ct);
    }

    public async Task<bool> RejectSubmissionAsync(int submissionId, CancellationToken ct)
    {
        var submission = await _dbContext.ocrSubmissions.Include(s => s.citizen)
            .FirstOrDefaultAsync(s => s.submissionId == submissionId, ct);

        if (submission == null)
        {
            return false;
        }

        if (submission.status != OcrSubmissionStatus.Pending)
        {
            throw new InvalidOperationException("Only pending submissions can be rejected.");
        }

        submission.status = OcrSubmissionStatus.Rejected;
        submission.citizen.submissionStatus = SubmissionStatus.Rejected;
        _dbContext.ocrSubmissions.Update(submission);
        _dbContext.citizens.Update(submission.citizen);
        await _dbContext.SaveChangesAsync(ct);

        return true;
    }

    public void Update(OcrSubmission submission)
    {
        _dbContext.ocrSubmissions.Update(submission);
    }
}
