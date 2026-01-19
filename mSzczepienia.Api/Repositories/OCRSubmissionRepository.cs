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
    public OCRSubmissionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public async Task AddOcrSubmissionAsync(string firebaseUid, string directoryUrl, List<IFormFile> files, CancellationToken ct)
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

        var newSubmission = new OcrSubmission
        {
            citizenId = citizen.citizenId,
            submissionDate = DateTime.UtcNow,
            status = OcrSubmissionStatus.Uploaded,
            photoUrl = directoryUrl,
        };

        _dbContext.ocrSubmissions.Add(newSubmission);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<OcrSubmission?> GetByIdAsync(int submissionId, CancellationToken ct)
    {
        return await _dbContext.ocrSubmissions
            .FirstOrDefaultAsync(s => s.submissionId == submissionId, ct);
    }

    public void Update(OcrSubmission submission)
    {
        _dbContext.ocrSubmissions.Update(submission);
    }

    public async Task<string> DeleteOcrSubmissionAsync(string firebaseUid, CancellationToken ct)
    {
        var citizen = await _dbContext.citizens
            .AsNoTracking()
            .Include(c => c.account)
            .FirstOrDefaultAsync(c => c.account.firebaseUid == firebaseUid, ct);
        if (citizen == null)
        {
            throw new InvalidOperationException("Citizen not found for the provided Firebase UID.");
        }

        var submission = await _dbContext.ocrSubmissions
            .OrderByDescending(s => s.submissionDate)
            .FirstOrDefaultAsync(s => s.citizenId == citizen.citizenId, ct);
        if (submission == null)
        {
            throw new InvalidOperationException("OCR submission not found for the citizen.");
        }

        _dbContext.ocrSubmissions.Remove(submission);
        await _dbContext.SaveChangesAsync(ct);
        
        return submission.photoUrl;
    }

    public async Task UpdateOcrSubmissionWithEntries(string firebaseUid, List<VaccinationEntryDto> entries, CancellationToken ct)
    {
        var citizen = await _dbContext.citizens
            .Include(c => c.account)
            .FirstOrDefaultAsync(c => c.account.firebaseUid == firebaseUid, ct);
        if (citizen == null)
        {
            throw new InvalidOperationException("Citizen not found for the provided Firebase UID.");
        }

        var submission = await _dbContext.ocrSubmissions
            .OrderByDescending(s => s.submissionDate)
            .FirstOrDefaultAsync(s => s.citizenId == citizen.citizenId, ct);
        if (submission == null)
        {
            throw new InvalidOperationException("OCR submission not found for the citizen.");
        }

        // Update submission status and date
        submission.status = OcrSubmissionStatus.Pending;
        submission.submissionDate = DateTime.UtcNow;
        citizen.submissionStatus = SubmissionStatus.Pending;

        // Add vaccination entries
        foreach (var entryDto in entries)
        {
            var vaccine = await _dbContext.vaccineDefinitions
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.vaccineName == entryDto.VaccineName, ct);

            DateTime? date = entryDto.Date;
            if (entryDto.Date == new DateTime(1900, 1, 1))
            {
                date = null;
            }

            var vaccinationEntry = new VaccinationEntry
            {
                ocrSubmissionId = submission.submissionId,
                vaccineNameOcr = entryDto.VaccineNameOcr ?? string.Empty,
                dateOcr = entryDto.DateOcr ?? string.Empty,
                batchOcr = entryDto.BatchOcr ?? string.Empty,
                vaccineId = vaccine?.vaccineId,
                date = date,
                batch = entryDto.Batch,
                status = entryDto.MapStringToEnum(),
            };
            _dbContext.vaccinationEntries.Add(vaccinationEntry);
        }

        await _dbContext.SaveChangesAsync(ct);
    }

}
