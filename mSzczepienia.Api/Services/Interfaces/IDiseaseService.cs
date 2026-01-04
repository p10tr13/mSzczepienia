using mSzczepienia.Shared.Models;
using mSzczepienia.Shared.DTO.Diseases;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IDiseaseService
{
    Task<List<DiseaseDto>> GetDiseasesAutocompleteAsync(string query, int count, CancellationToken ct);
}
