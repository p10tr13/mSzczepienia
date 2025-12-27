using mSzczepienia.Shared.DTO.Accounts;

namespace mSzczepienia.Client.Services.Interfaces;

public interface IAccountClient
{
    Task<ProfileDto?> GetProfileAsync(CancellationToken ct);
}
