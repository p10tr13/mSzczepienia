using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Services;

public class VaccineService : IVaccineService
{
    private readonly IVaccineRepository _vaccineRepository;
    private readonly IDiseaseRepository _diseaseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VaccineService(IVaccineRepository vaccineRepository, IDiseaseRepository diseaseRepository, IUnitOfWork unitOfWork)
    {
        _vaccineRepository = vaccineRepository;
        _diseaseRepository = diseaseRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<List<string>> GetManufacturersAutocompleteAsync(string? query, CancellationToken ct)
    {
        return _vaccineRepository.GetManufacturersAutocompleteAsync(query, ct);
    }

    public Task<List<string>> GetNamesAutocompleteAsync(string? query, CancellationToken ct)
    {
        return _vaccineRepository.GetNamesAutocompleteAsync(query, ct);
    }

    public async Task<PagedResult<VaccineDto>> GetVaccinesAsync(GetVaccinesQuery query, CancellationToken ct)
    {
        return await _vaccineRepository.GetVaccinesAsync(query, ct);
    }

    public async Task<VaccineDto?> GetVaccineByIdAsync(int id, CancellationToken ct)
    {
        return await _vaccineRepository.GetVaccineByIdAsync(id, ct);
    }

    public async Task CreateVaccineAsync(CreateVaccineDto dto, CancellationToken ct)
    {
        var newVaccine = new VaccineDefinition
        {
            vaccineName = dto.VaccineName,
            manufacturer = dto.Manufacturer,
            vaccineDescriptionEn = dto.VaccineDescriptionEn,
            vaccineDescriptionPl = dto.VaccineDescriptionPl,
            shortDescriptionEn = dto.ShortDescriptionEn,
            shortDescriptionPl = dto.ShortDescriptionPl,
            creationDate = DateTime.UtcNow,

            vaccineDiseases = new List<VaccineDisease>()
        };

        foreach (var diseaseDto in dto.Diseases)
        {
            Disease? diseaseEntity;

            if (diseaseDto.DiseaseId == 0)
            {
                diseaseEntity = new Disease
                {
                    diseaseNameEn = diseaseDto.DiseaseNameEn ?? "Unknown",
                    diseaseNamePl = diseaseDto.DiseaseNamePl ?? "Nieznana"
                };
            }
            else
            {
                diseaseEntity = await _diseaseRepository.GetByIdAsync(diseaseDto.DiseaseId, ct);

                if (diseaseEntity == null)
                {
                    continue;
                }
            }

            newVaccine.vaccineDiseases.Add(new VaccineDisease
            {
                disease = diseaseEntity,
            });
        }

        await _vaccineRepository.AddAsync(newVaccine, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
