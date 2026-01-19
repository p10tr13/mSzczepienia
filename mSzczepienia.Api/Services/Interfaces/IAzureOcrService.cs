using mSzczepienia.Shared.DTO.VaccinationEntries;

namespace mSzczepienia.Api.Services.Interfaces
{
    public interface IAzureOcrService
    {
        Task<List<VaccinationEntryOcrDto>> ProcessFilesAsync(List<IFormFile> files, CancellationToken ct);
    }
}