using mSzczepienia.Shared.Models;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Personnel;

namespace mSzczepienia.Api.Repositories.Interfaces;

public interface IPersonnelRepository
{
    Task AddAsync(Personnel personnel, CancellationToken ct);
    Task<PagedResult<PersonnelDto>> GetFacilityPersonnelAsync(GetPersonnelQuery query, CancellationToken ct);
    Task<int?> GetFacilityIdForWorkerByFirebaseUidAsync(string firebaseUid, CancellationToken ct);
    Task<Personnel?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken ct);
    Task<PagedResult<Personnel>> SearchPersonnelAsync(SearchPersonnelQuery query, CancellationToken ct);
    void Update(Personnel personnel);
}
