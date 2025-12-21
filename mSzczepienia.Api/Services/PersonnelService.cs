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
}
