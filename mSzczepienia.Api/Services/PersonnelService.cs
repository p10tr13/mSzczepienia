using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Api.Repositories.Interfaces;

namespace mSzczepienia.Api.Services;

public class PersonnelService : IPersonnelService
{
    private readonly IPersonnelRepository _personnelRepository;
    private readonly IFacilityRepository _facilityRepository;

    public PersonnelService(IPersonnelRepository personnelRepository, IFacilityRepository facilityRepository)
    {
        _personnelRepository = personnelRepository;
        _facilityRepository = facilityRepository;
    }

    public async Task<PagedResult<PersonnelDto>> GetPersonnelAsync(GetPersonnelQuery query, CancellationToken ct)
    {
        return await _personnelRepository.GetFacilityPersonnelAsync(query, ct);
    }

    public async Task<PagedResult<PersonnelDto>> SearchPersonnelAsync(SearchPersonnelQuery query, CancellationToken ct)
    {
        var pagedResult = await _personnelRepository.SearchPersonnelAsync(query, ct);

        var personnelDtos = pagedResult.Items.Select(p => new PersonnelDto
        {
            Id = p.personnelId,
            AccountId = p.accountId,
            Name = p.account.name,
            Surname = p.account.surname,
            FacilityId = p.facilityId,
            PwzNumber = p.pwzNumber,
            Position = p.position.ToString(),
            Email = p.account.email,
            DateOfBirth = p.account.dateOfBirth
        }).ToList();

        return new PagedResult<PersonnelDto>
        {
            Items = personnelDtos,
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<DetailedPersonnelDto> GetDetailedPersonnelByIdAsync(int id, CancellationToken ct)
    {
        var personnel = await _personnelRepository.GetByIdAsync(id, ct);

        if (personnel == null)
        {
            throw new KeyNotFoundException($"Personnel with ID {id} not found.");
        }

        var facility = await _facilityRepository.GetByIdAsync(personnel.facilityId, ct);

        if (facility == null)
        {
            throw new KeyNotFoundException($"Facility with ID {personnel.facilityId} not found.");
        }

        return new DetailedPersonnelDto
        {
            Id = personnel.accountId,
            Name = personnel.account.name,
            Surname = personnel.account.surname,
            Email = personnel.account.email,
            CreationDate = personnel.account.creationDate,
            DateOfBirth = personnel.account.dateOfBirth,
            Role = personnel.account.role,
            PwzNumber = personnel.pwzNumber,
            Position = personnel.position,
            FacilityId = personnel.facilityId,
            FacilityName = facility.facilityName,
            FacilityCity = facility.city,
            FacilityStreet = facility.street,
            FacilityPostalCode = facility.postalCode
        };
    }
}
