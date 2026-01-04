using mSzczepienia.Shared.DTO.Diseases;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IDiseaseClient
{
    Task<List<DiseaseDto>> GetDiseasesAutocompleteAsync(string query, int count, CancellationToken ct);
}
