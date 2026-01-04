using mSzczepienia.Shared.Models;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Diseases;

namespace mSzczepienia.Api.Services;

public class DiseaseService : IDiseaseService
{
    private readonly IDiseaseRepository _diseaseRepository;

    public DiseaseService(IDiseaseRepository diseaseRepository)
    {
        _diseaseRepository = diseaseRepository;
    }

    public async Task<List<DiseaseDto>> GetDiseasesAutocompleteAsync(string query, int count, CancellationToken ct)
    {
        var diseases = await _diseaseRepository.GetByNameAsNoTrackingAsync(query, ct);

        if (diseases == null || !diseases.Any())
        {
            return new List<DiseaseDto>();
        }

        return diseases.Take(count).Select(d => new DiseaseDto
        {
            DiseaseId = d.diseaseId,
            DiseaseNameEn = d.diseaseNameEn,
            DiseaseNamePl = d.diseaseNamePl
        }).ToList();
    }
}
