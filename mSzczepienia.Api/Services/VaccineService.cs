using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Vaccines;
using mSzczepienia.Shared.DTO.Diseases;
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

    public async Task<DetailedVaccineDto?> GetDetailedVaccineByIdAsync(int id, CancellationToken ct)
    {
        var vaccine = await _vaccineRepository.GetVaccineDefinitionByIdAsync(id, ct);

        if (vaccine == null)
        {
            return null;
        }

        return new DetailedVaccineDto
        {
            Id = vaccine.vaccineId,
            VaccineName = vaccine.vaccineName,
            Manufacturer = vaccine.manufacturer,
            VaccineDescriptionEn = vaccine.vaccineDescriptionEn,
            VaccineDescriptionPl = vaccine.vaccineDescriptionPl,
            ShortDescriptionEn = vaccine.shortDescriptionEn,
            ShortDescriptionPl = vaccine.shortDescriptionPl,
            Diseases = vaccine.vaccineDiseases.Select(vd => new DiseaseDto
            {
                DiseaseId = vd.disease.diseaseId,
                DiseaseNameEn = vd.disease.diseaseNameEn,
                DiseaseNamePl = vd.disease.diseaseNamePl
            }).ToList()
        };
    }

    public async Task UpdateVaccineAsync(int id, CreateVaccineDto dto, CancellationToken ct)
    {
        var vaccine = await _vaccineRepository.GetVaccineDefinitionByIdAsync(id, ct);
        if (vaccine == null)
        {
            throw new Exception("Vaccine not found");
        }

        vaccine.vaccineName = dto.VaccineName;
        vaccine.manufacturer = dto.Manufacturer;
        vaccine.vaccineDescriptionEn = dto.VaccineDescriptionEn;
        vaccine.vaccineDescriptionPl = dto.VaccineDescriptionPl;
        vaccine.shortDescriptionEn = dto.ShortDescriptionEn;
        vaccine.shortDescriptionPl = dto.ShortDescriptionPl;
        
        _vaccineRepository.RemoveRangeVaccineDisease(vaccine.vaccineDiseases);
        vaccine.vaccineDiseases = new List<VaccineDisease>();

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

            vaccine.vaccineDiseases.Add(new VaccineDisease
            {
                disease = diseaseEntity,
            });
        }

        _vaccineRepository.Update(vaccine);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<List<VaccineNameDto>> SearchVaccineNamesAsync(string? query, CancellationToken ct)
    {
        return await _vaccineRepository.SearchVaccineNamesAsync(query, ct);
    }

}
