using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO.Users;
using mSzczepienia.Api.Repositories.Interfaces;
using FirebaseAdmin.Auth;
using mSzczepienia.Shared.Models;
using mSzczepienia.Shared.Enums;

namespace mSzczepienia.Api.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPersonnelRepository _personnelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserManagementService(IAccountRepository accountRepository, IPersonnelRepository personnelRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _personnelRepository = personnelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateUserAsync(CreateUserDto createUserDto, CancellationToken ct)
    {
        string createdUid = "";
            
        try
        {
            if (createUserDto.Role != Role.HealthWorker && createUserDto.Role != Role.Admin)
            {
                throw new ArgumentException("Unsupported role");
            }

            var userArgs = new UserRecordArgs()
            {
                Email = createUserDto.Email,
                EmailVerified = true,
                Password = createUserDto.Password,
                DisplayName = $"{createUserDto.Name} {createUserDto.Surname}",
                Disabled = false,
            };

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs, ct);
            createdUid = userRecord.Uid;

            var claims = new Dictionary<string, object>();

            if (createUserDto.Role == Role.Admin)
            {
                claims.Add("role", "admin");
            }
            else if (createUserDto.Role == Role.HealthWorker)
            {
                claims.Add("role", "worker");
            }

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(createdUid, claims, ct);

            var newAccount = new Account
            {
                firebaseUid = createdUid,
                email = createUserDto.Email,
                role = createUserDto.Role,
                name = createUserDto.Name,
                surname = createUserDto.Surname,
                creationDate = DateTime.UtcNow,
                dateOfBirth = createUserDto.DateOfBirth ?? throw new ArgumentException("Date of birth is required"),
            };

            await _accountRepository.AddAsync(newAccount, ct);

            if (createUserDto.Role == Role.HealthWorker)
            {
                var newPersonnel = new Personnel
                {
                    facilityId = createUserDto.FacilityId ?? throw new ArgumentException("Facility is required for HealthWorker"),
                    pwzNumber = createUserDto.PwzNumber ?? throw new ArgumentException("PWZ is required for HealthWorker"),
                    position = createUserDto.Position ?? throw new ArgumentException("Position is required for HealthWorker"),
                    account = newAccount
                };
                await _personnelRepository.AddAsync(newPersonnel, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (FirebaseAuthException ex) when (ex.ErrorCode == FirebaseAdmin.ErrorCode.AlreadyExists)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }
        catch (Exception)
        {
            if (!string.IsNullOrEmpty(createdUid))
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(createdUid, ct);
            }
            throw;
        }
    }
}
