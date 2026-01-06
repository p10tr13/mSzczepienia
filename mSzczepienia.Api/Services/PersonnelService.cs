using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Api.Repositories.Interfaces;

namespace mSzczepienia.Api.Services;

public class PersonnelService : IPersonnelService
{
    private readonly IPersonnelRepository _personnelRepository;

    public PersonnelService(IPersonnelRepository personnelRepository)
    {
        _personnelRepository = personnelRepository;
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
}
