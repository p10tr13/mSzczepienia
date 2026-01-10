using FirebaseAdmin.Auth;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Admins;
using mSzczepienia.Shared.DTO.Personnel;
using mSzczepienia.Shared.DTO.Queries;

namespace mSzczepienia.Api.Services;

public class PersonnelService : IPersonnelService
{
    private readonly IPersonnelRepository _personnelRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PersonnelService(IPersonnelRepository personnelRepository, IFacilityRepository facilityRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _personnelRepository = personnelRepository;
        _facilityRepository = facilityRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
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

    public async Task UpdatePersonnelAsync(int id, UpdatePersonnelDto updateDto, CancellationToken ct)
    {
        var personnel = await _personnelRepository.GetByIdAsync(id, ct);

        if (personnel == null)
        {
            throw new KeyNotFoundException($"Personnel with ID {id} not found.");
        }

        bool emailChanged = !string.Equals(personnel.account.email, updateDto.Email, StringComparison.OrdinalIgnoreCase);
        if (emailChanged)
        {
            var existingAccountWithEmail = await _accountRepository.GetByEmailAsync(updateDto.Email, ct);
            if (existingAccountWithEmail != null && existingAccountWithEmail.accountId != personnel.accountId)
            {
                throw new InvalidOperationException("The provided email is already in use by another account.");
            }

            try
            {
                var args = new UserRecordArgs
                {
                    Uid = personnel.account.firebaseUid,
                    Email = updateDto.Email,
                };

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, ct);
                personnel.account.email = updateDto.Email;
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

        personnel.account.name = updateDto.Name;
        personnel.account.surname = updateDto.Surname;
        personnel.account.dateOfBirth = updateDto.DateOfBirth;
        personnel.pwzNumber = updateDto.PwzNumber;
        personnel.position = updateDto.Position;

        _personnelRepository.Update(personnel);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
