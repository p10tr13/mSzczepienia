using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Citizens;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;
using System.Linq;

namespace mSzczepienia.Api.Repositories
{
    public class CitizenRepository : ICitizenRepository
    {
        private readonly AppDbContext _dbContext;
        public CitizenRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task RegisterDeviceTokenAsync(string firebaseUid, string deviceToken, string language, CancellationToken ct)
        {
            var citizen = await _dbContext.citizens
                .Include(c => c.account)
                .Include(c => c.deviceTokens)
                .Where(c => c.account.firebaseUid == firebaseUid)
                .FirstOrDefaultAsync(ct);

            if (citizen == null)
            {
                return;
            }

            var existingToken = citizen.deviceTokens
                .FirstOrDefault(t => t.deviceToken == deviceToken);

            if (existingToken != null)
            {
                existingToken.lastUpdated = DateTime.UtcNow;
                existingToken.language = language;
            }
            else
            {
                var newToken = new CitizenDeviceToken
                {
                    deviceToken = deviceToken,
                    citizenId = citizen.citizenId,
                    language = language,
                    lastUpdated = DateTime.UtcNow
                };

                citizen.deviceTokens.Add(newToken);
            }
            await _dbContext.SaveChangesAsync(ct);
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

        public async Task<CitizenDetailsDto?> GetCitizenDetailsByIdAsync(int citizenId, CancellationToken ct)
        {
            return await _dbContext.citizens
                .Where(c => c.citizenId == citizenId)
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

        public async Task<PagedResult<SimpleCitizenDto>> SearchCitizensAsync(GetCitizensQuery query, int? facilityId, CancellationToken ct)
        {
            var queryable = _dbContext.citizens.Include(c => c.account).AsNoTracking().AsQueryable();

            if (query.SearchInWorkersFacilityOnly && facilityId.HasValue)
            {
                queryable = queryable.Where(c => c.facilityId == facilityId);
            }

            if (!string.IsNullOrEmpty(query.Name))
            {
                queryable = queryable.Where(c => c.account.name.ToLower().Contains(query.Name.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(query.Surname))
            {
                queryable = queryable.Where(c => c.account.surname.ToLower().Contains(query.Surname.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(query.Pesel))
            {
                queryable = queryable.Where(c => c.pesel.Contains(query.Pesel));
            }

            var totalCount = await queryable.CountAsync(ct);
            var items = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new SimpleCitizenDto
            {
                Id = c.citizenId,
                Name = c.account.name,
                Surname = c.account.surname,
                Pesel = c.pesel,
                DateOfBirth = DateTime.SpecifyKind(c.account.dateOfBirth, DateTimeKind.Unspecified),
                SubmissionStatus = c.submissionStatus.ToString(),
            })
            .ToListAsync(ct);
            return new PagedResult<SimpleCitizenDto>
            {
                TotalCount = totalCount,
                Items = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<CitizenSubmissionStatusDto> GetCitizenSubmissionStatusAsync(string firebaseUid, CancellationToken ct)
        {
            var citizenStatus = await _dbContext.citizens
                .Where(c => c.account.firebaseUid == firebaseUid)
                .Select(c => c.submissionStatus)
                .FirstOrDefaultAsync(ct);

            var citizenStatusDto = new CitizenSubmissionStatusDto
            {
                submissionStatus = citizenStatus.ToString()
            };

            return citizenStatusDto;
        }

        public async Task<int?> GetFacilityIdByCitizenIdAsync(int citizenId, CancellationToken ct)
        {
            return await _dbContext.citizens
                .Where(c => c.citizenId == citizenId)
                .Select(c => (int?)c.facilityId)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<Citizen?> GetByIdAsync(int citizenId, CancellationToken ct)
        {
            return await _dbContext.citizens
                .Include(c => c.account)
                .FirstOrDefaultAsync(c => c.citizenId == citizenId, ct);
        }

        public void Update(Citizen citizen)
        {
            _dbContext.citizens.Update(citizen);
        }
    }
}