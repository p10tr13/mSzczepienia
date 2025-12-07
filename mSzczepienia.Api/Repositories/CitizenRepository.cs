using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO;
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

        public async Task<CitizenDetailsDto?> GetCitizenDetailsByUidAsync(string firebaseUid)
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

                    peselHash = c.peselHash,
                    submissionStatus = c.submissionStatus.ToString(),

                    facilityName = c.medicalFacility.facilityName,
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddCitizenAsync(Citizen citizen)
        {
            await _dbContext.citizens.AddAsync(citizen);
            await _dbContext.SaveChangesAsync();
        }
    }
}