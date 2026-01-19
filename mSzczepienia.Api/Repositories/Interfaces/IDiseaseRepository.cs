using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IDiseaseRepository
{
    Task<Disease?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<Disease>?> GetByNameAsNoTrackingAsync(string name, CancellationToken ct);
    Task AddAsync(Disease disease, CancellationToken ct);
}
