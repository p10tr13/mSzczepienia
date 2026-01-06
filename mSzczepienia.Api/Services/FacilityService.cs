using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Facilities;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Services;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FacilityService(IFacilityRepository facilityRepository, IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<string>> GetCitiesAutocompleteAsync(string? query, CancellationToken ct)
    {
        return await _facilityRepository.GetCitiesAutocompleteAsync(query, ct);
    }

    public async Task<List<FacilityListDto>> GetFacilityNamesAutocompleteAsync(string? query, CancellationToken ct)
    {
        var facilityNames = await _facilityRepository.GetFacilityNamesAutocompleteAsync(query, ct);

        return facilityNames;
    }

    public async Task<PagedResult<SearchFacilityDto>> SearchFacilitiesAsync(GetFacilitiesQuery query, CancellationToken ct)
    {
        var pagedFacilities = await _facilityRepository.SearchFacilitiesAsync(query, ct);

        var dtos = pagedFacilities.Items.Select(f => new SearchFacilityDto
        {
            Id = f.facilityId,
            Name = f.facilityName,
            Street = f.street,
            City = f.city,
            PostalCode = f.postalCode,
            Nip = f.nip
        }).ToList();

        return new PagedResult<SearchFacilityDto>
        {
            Items = dtos,
            TotalCount = pagedFacilities.TotalCount,
            PageNumber = pagedFacilities.PageNumber,
            PageSize = pagedFacilities.PageSize
        };
    }

    public async Task<FacilityDto?> GetFacilityByIdAsync(int id, CancellationToken ct)
    {
        var f = await _facilityRepository.GetByIdAsNoTrackingAsync(id, ct);

        if (f == null)
            return null;

        return new FacilityDto
        {
            Id = f.facilityId,
            Name = f.facilityName,
            Street = f.street,
            City = f.city,
            PostalCode = f.postalCode,
            Nip = f.nip,
            PhoneNumber = f.phoneNumber,
            CreationDate = f.creationDate,
            Country = f.country
        };
    }

    public async Task<int> CreateFacilityAsync(CreateFacilityDto dto, CancellationToken ct)
    {
        var facility = new MedicalFacility
        {
            facilityName = dto.FacilityName,
            nip = dto.Nip,
            phoneNumber = dto.PhoneNumber,
            street = dto.Street,
            postalCode = dto.PostalCode,
            city = dto.City,
            country = dto.Country ?? "Polska",
            creationDate = DateTime.UtcNow
        };

        await _facilityRepository.AddAsync(facility, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return facility.facilityId;
    }

    public async Task UpdateFacilityAsync(int id, CreateFacilityDto dto, CancellationToken ct)
    {
        var facility = await _facilityRepository.GetByIdAsync(id, ct);
        if (facility == null)
            throw new KeyNotFoundException("Facility not found.");

        facility.facilityName = dto.FacilityName;
        facility.nip = dto.Nip;
        facility.phoneNumber = dto.PhoneNumber;
        facility.street = dto.Street;
        facility.postalCode = dto.PostalCode;
        facility.city = dto.City;
        facility.country = dto.Country ?? facility.country;

        _facilityRepository.Update(facility);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
