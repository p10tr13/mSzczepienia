using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.OCRSubmissions;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Enums;

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
            .OrderByDescending(s => s.submissionDate)
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
}
