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
                AcceptedCount = g.Count(s => s.status == SubmissionStatus.Accepted),
                NotReviewedCount = g.Count(s => s.status == SubmissionStatus.Pending),
                RejectedCount = g.Count(s => s.status == SubmissionStatus.Rejected)
            })
            .FirstOrDefaultAsync(ct) ?? new DashboardStatsDto();
    }
}
