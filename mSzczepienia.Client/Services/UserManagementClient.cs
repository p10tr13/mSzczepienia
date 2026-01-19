using mSzczepienia.Shared.DTO.Users;
using mSzczepienia.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace mSzczepienia.Client.Services;

public class UserManagementClient : IUserManagementClient
{
    private readonly HttpClient _httpClient;

    public UserManagementClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateUserAsync(CreateUserDto createUserDto)
    {
        if (createUserDto.DateOfBirth == null)
        {
            throw new ArgumentNullException("Date of birth is invalid");
        }
        createUserDto.DateOfBirth = DateTime.SpecifyKind(createUserDto.DateOfBirth.Value, DateTimeKind.Unspecified);
        var response = await _httpClient.PostAsJsonAsync("api/users-management/create", createUserDto);
        response.EnsureSuccessStatusCode();
        return response.IsSuccessStatusCode;
    }

}
