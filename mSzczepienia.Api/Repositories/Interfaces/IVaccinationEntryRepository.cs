using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IVaccinationEntryRepository
{
    Task<List<VaccinationEntry>> GetBySubmissionIdAsync(int ocrSubmissionId, CancellationToken ct);
    Task AddAsync(VaccinationEntry entry, CancellationToken ct);
    void Update(VaccinationEntry entry);
}
