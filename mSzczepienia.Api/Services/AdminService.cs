using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.DTO.Admins;
using FirebaseAdmin.Auth;

namespace mSzczepienia.Api.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IAdminRepository adminRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<AccountDto>> SearchAdminsAsync(SearchAdminQuery query, CancellationToken ct)
    {
        var accounts = await _adminRepository.SearchAdminsAsync(query, ct);

        var items = accounts.Items.Select(a => new AccountDto
        {
            Id = a.accountId,
            Name = a.name,
            Surname = a.surname,
            Email = a.email
        }).ToList();

        return new PagedResult<AccountDto>
        {
            Items = items,
            TotalCount = accounts.TotalCount,
            PageNumber = accounts.PageNumber,
            PageSize = accounts.PageSize
        };
    }

    public async Task<DetailedAccountDto> GetAdminByIdAsync(Guid adminId, CancellationToken ct)
    {
        var admin = await _accountRepository.GetByIdAsync(adminId, ct);

        if (admin == null || admin.role != Role.Admin)
        {
            throw new KeyNotFoundException("Admin not found.");
        }

        return new DetailedAccountDto
        {
            Id = admin.accountId,
            Name = admin.name,
            Surname = admin.surname,
            Email = admin.email,
            CreationDate = admin.creationDate,
            DateOfBirth = admin.dateOfBirth,
            Role = admin.role
        };
    }

    public async Task UpdateAdminAsync(Guid id, UpdateAdminDto updateAdminDto, CancellationToken ct)
    {
        var admin = await _accountRepository.GetByIdAsync(id, ct);

        if (admin == null || admin.role != Role.Admin)
        {
            throw new KeyNotFoundException("Admin not found.");
        }

        bool emailChanged = !string.Equals(admin.email, updateAdminDto.Email, StringComparison.OrdinalIgnoreCase);
        if (emailChanged)
        {
            var existingAccountWithEmail = await _accountRepository.GetByEmailAsync(updateAdminDto.Email, ct);
            if (existingAccountWithEmail != null && existingAccountWithEmail.accountId != id)
            {
                throw new InvalidOperationException("The provided email is already in use by another account.");
            }

            try
            {
                var args = new UserRecordArgs
                {
                    Uid = admin.firebaseUid,
                    Email = updateAdminDto.Email,
                };

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, ct);
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
                {                    
                    throw new InvalidOperationException("The provided email is already in use in Firebase Authentication.", ex);
                }
                throw;
            }
        }

        admin.name = updateAdminDto.Name;
        admin.surname = updateAdminDto.Surname;
        admin.email = updateAdminDto.Email;
        admin.dateOfBirth = updateAdminDto.DateOfBirth;

        _accountRepository.Update(admin);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
