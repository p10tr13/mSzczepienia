using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO.Stats;
using mSzczepienia.Shared.Enums;

namespace mSzczepienia.Api.Repositories;

public class StatsRepository : IStatsRepository
{
    private readonly AppDbContext _dbContext;

    public StatsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatsDto> GetAdminStatsAsync(CancellationToken ct)
    {
        return await _dbContext.ocrSubmissions.AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new DashboardStatsDto
            {
                TotalBooklets = g.Count(),
                AcceptedCount = g.Count(s => s.status == OcrSubmissionStatus.Accepted),
                NotReviewedCount = g.Count(s => s.status == OcrSubmissionStatus.Pending),
                RejectedCount = g.Count(s => s.status == OcrSubmissionStatus.Rejected)
            })
            .FirstOrDefaultAsync(ct) ?? new DashboardStatsDto();
    }

    public async Task<DashboardStatsDto> GetFacilityStatsAsync(int facilityId, CancellationToken ct)
    {
        return await _dbContext.ocrSubmissions
            .AsNoTracking()
            .Where(s => s.citizen.medicalFacility.facilityId == facilityId)
            .GroupBy(_ => 1)
            .Select(g => new DashboardStatsDto
            {
                TotalBooklets = g.Count(),
                AcceptedCount = g.Count(s => s.status == OcrSubmissionStatus.Accepted),
                NotReviewedCount = g.Count(s => s.status == OcrSubmissionStatus.Pending),
                RejectedCount = g.Count(s => s.status == OcrSubmissionStatus.Rejected)
            })
            .FirstOrDefaultAsync(ct) ?? new DashboardStatsDto {TotalBooklets = 0, AcceptedCount = 0, NotReviewedCount = 0, RejectedCount = 0 }
        ;
    }

    public async Task<DashboardStatsDto> GetWorkerStatsByFirebaseUidAsync(string firebaseUid, CancellationToken ct)
    {
        var personnelQuery = _dbContext.personnel.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(firebaseUid))
        {
            personnelQuery = personnelQuery.Where(p => p.account.firebaseUid == firebaseUid);
        }
        else
        {
            return new DashboardStatsDto();
        }

        var personnel = await personnelQuery.FirstOrDefaultAsync(ct);
        if (personnel == null)
        {
            return new DashboardStatsDto();
        }

        return await _dbContext.ocrSubmissions
            .AsNoTracking()
            .Where(s => s.citizen.medicalFacility.facilityId == personnel.facilityId)
            .GroupBy(_ => 1)
            .Select(g => new DashboardStatsDto
            {
                TotalBooklets = g.Count(),
                AcceptedCount = g.Count(s => s.status == OcrSubmissionStatus.Accepted),
                NotReviewedCount = g.Count(s => s.status == OcrSubmissionStatus.Pending),
                RejectedCount = g.Count(s => s.status == OcrSubmissionStatus.Rejected)
            })
            .FirstOrDefaultAsync(ct) ?? new DashboardStatsDto { TotalBooklets = 0, AcceptedCount = 0, NotReviewedCount = 0, RejectedCount = 0 };
    }
}
