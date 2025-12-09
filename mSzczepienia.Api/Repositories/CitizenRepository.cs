using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories
{
    public class CitizenRepository: ICitizenRepository
    {
        private readonly AppDbContext _dbContext;
        public CitizenRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<CitizenDetailsDto?> GetCitizenDetailsByUidAsync(string firebaseUid, CancellationToken ct)
        {
            return await _dbContext.citizens
                .Where(c => c.account.firebaseUid == firebaseUid)
                .Select(c => new CitizenDetailsDto
                {
                    name = c.account.name,
                    surname = c.account.surname,
                    email = c.account.email,
                    dateOfBirth = c.account.dateOfBirth,
                    creationDate = c.account.creationDate,
                    pesel = c.pesel,
                    submissionStatus = c.submissionStatus.ToString(),
                    facilityName = c.medicalFacility.facilityName,
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task AddCitizenAsync(string firebaseUid, CitizenDetailsDto dto, CancellationToken ct)
        {
            var existingFacility = await _dbContext.medicalFacilities
                .Where(f => f.facilityName == dto.facilityName)
                .FirstOrDefaultAsync(ct);

            if (existingFacility == null)
            {
                throw new InvalidOperationException($"Facility has not been found: {dto.facilityName}");
            }

            var citizen = new Citizen
            {
                pesel = dto.pesel,
                submissionStatus = SubmissionStatus.NotUploaded,
                account = new Account
                {
                    firebaseUid = firebaseUid,
                    name = dto.name,
                    surname = dto.surname,
                    email = dto.email,
                    dateOfBirth = dto.dateOfBirth,
                    creationDate = DateTime.UtcNow
                },
                medicalFacility = existingFacility
            };

            await _dbContext.citizens.AddAsync(citizen);
            await _dbContext.SaveChangesAsync();
        }
    }
}