using mSzczepienia.Shared.DTO.Users;

namespace mSzczepienia.Api.Services.Interfaces;

public interface IUserManagementService
{
    Task CreateUserAsync(CreateUserDto dto, CancellationToken ct);
}
