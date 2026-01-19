using mSzczepienia.Shared.DTO.Users;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IUserManagementClient
{
    public Task<bool> CreateUserAsync(CreateUserDto createUserDto);
}
