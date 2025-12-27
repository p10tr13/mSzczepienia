using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO.Accounts;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories
{
    public class AccountProfileRepository : IAccountProfileRepository
    {
        private readonly AppDbContext _dbContext;

        public AccountProfileRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProfileDto?> GetProfileByFirebaseUidAsync(string firebaseUid, CancellationToken ct)
        {
            // Try to find personnel (worker) first
            var personnel = await _dbContext.personnel
                .Include(p => p.account)
                .Include(p => p.medicalFacility)
                .FirstOrDefaultAsync(p => p.account.firebaseUid == firebaseUid, ct);

            if (personnel != null)
            {
                return new ProfileDto
                {
                    Name = personnel.account.name,
                    Surname = personnel.account.surname,
                    Email = personnel.account.email,
                    DateOfBirth = personnel.account.dateOfBirth,
                    CreationDate = personnel.account.creationDate,
                    Role = personnel.account.role.ToString(),
                    PwzNumber = personnel.pwzNumber,
                    Position = personnel.position.ToString(),
                    FacilityName = personnel.medicalFacility.facilityName
                };
            }

            // Otherwise try to find plain account (admin or other)
            var account = await _dbContext.accounts
                .FirstOrDefaultAsync(a => a.firebaseUid == firebaseUid, ct);

            if (account == null)
                return null;

            return new ProfileDto
            {
                Name = account.name,
                Surname = account.surname,
                Email = account.email,
                DateOfBirth = account.dateOfBirth,
                CreationDate = account.creationDate,
                Role = account.role.ToString()
            };
        }
    }
}
