using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories
{
    public class VaccinationRecordRepository : IVaccinationRecordRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly INotificationService _notificationService;

        public VaccinationRecordRepository(AppDbContext dbContext, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        public async Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordOverviewAsync(String firebaseUid, GetVaccinationRecordOverviewQuery query, CancellationToken ct)
        {
            var citizenId = await _dbContext.citizens
                .AsNoTracking()
                .Where(c => c.account.firebaseUid == firebaseUid)
                .Select(c => c.citizenId)
                .FirstOrDefaultAsync(ct);

            var baseQuery = _dbContext.vaccinationRecords
                .AsNoTracking()
                .Where(x => x.citizenId == citizenId);

            var groupedQuery = baseQuery
                .GroupBy(x => new { x.vaccineDefinition.vaccineId, x.vaccineDefinition.vaccineName, x.vaccineDefinition.shortDescriptionEn, x.vaccineDefinition.shortDescriptionPl })
                .Select(g => new VaccinationRecordOverviewDto
                {
                    id = g.Key.vaccineId,
                    vaccineName = g.Key.vaccineName,
                    shortDescriptionEn = g.Key.shortDescriptionEn,
                    shortDescriptionPl = g.Key.shortDescriptionPl,
                    vaccinationDate = g.Max(x => x.vaccinationDate),
                    dosesNumber = g.Count()
                });

            var sortField = query.sortingField?.ToLower() ?? "vaccinationdate";
            var isAsc = query.sortingDirection?.ToLower() == "asc";

            groupedQuery = sortField switch
            {
                "vaccinename" => isAsc
                    ? groupedQuery.OrderBy(x => x.vaccineName)
                    : groupedQuery.OrderByDescending(x => x.vaccineName),

                _ => isAsc
                    ? groupedQuery.OrderBy(x => x.vaccinationDate)
                    : groupedQuery.OrderByDescending(x => x.vaccinationDate)
            };

            var totalCount = await groupedQuery.CountAsync(ct);

            var items = await groupedQuery
                .Skip((query.pageNumber - 1) * query.pageSize)
                .Take(query.pageSize)
                .ToListAsync(ct);

            return new PagedResult<VaccinationRecordOverviewDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.pageNumber,
                PageSize = query.pageSize
            };
        }

        public async Task<List<VaccinationRecordDetailsDto>> GetVaccinationRecordsDetailsAsync(string firebaseUid, GetVaccinationRecordDetailsQuery query, CancellationToken ct)
        {
            var recordsQuery = _dbContext.vaccinationRecords
                .AsNoTracking()
                .Where(x => x.citizen.account.firebaseUid == firebaseUid && x.vaccineId == query.vaccineId);


            var resultQuery = recordsQuery.Select(x => new VaccinationRecordDetailsDto
            {
                vaccineId = x.vaccineId,
                vaccineName = x.vaccineDefinition.vaccineName,
                manufacturer = x.vaccineDefinition.manufacturer,
                vaccineDescriptionPl = x.vaccineDefinition.vaccineDescriptionPl,
                vaccineDescriptionEn = x.vaccineDefinition.vaccineDescriptionEn,
                shortDescriptionPl = x.vaccineDefinition.shortDescriptionPl,
                shortDescriptionEn = x.vaccineDefinition.shortDescriptionEn,
                creationDate = x.vaccineDefinition.creationDate,
                diseasesEn = x.vaccineDefinition.vaccineDiseases.Select(vd => vd.disease.diseaseNameEn).ToList(),
                diseasesPl = x.vaccineDefinition.vaccineDiseases.Select(vd => vd.disease.diseaseNamePl).ToList(),
                vaccinationDate = x.vaccinationDate,
                nextVaccinationDate = x.nextVaccinationDate,
                doseNumber = x.doseNumber,
                batch = x.batch,
                remarks = x.remarks
            });
            return await resultQuery.ToListAsync(ct);
        }

        public async Task<PagedResult<VaccinationRecordSimpleListItemDto>> GetVaccinationRecordsByCitizenIdAsync(GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct)
        {
            var baseQuery = _dbContext.vaccinationRecords.AsNoTracking().Include(x => x.vaccineDefinition).Where(x => x.citizen.citizenId == query.CitizenId);

            var sortField = query.SortingField?.ToLower() ?? "vaccinationdate";
            var isAsc = query.SortingDirection?.ToLower() == "asc";

            baseQuery = sortField switch
            {
                "vaccinename" => isAsc ? baseQuery.OrderBy(x => x.vaccineDefinition.vaccineName) : baseQuery.OrderByDescending(x => x.vaccineDefinition.vaccineName),

                _ => isAsc ? baseQuery.OrderBy(x => x.vaccinationDate) : baseQuery.OrderByDescending(x => x.vaccinationDate)
            };

            var totalCount = await baseQuery.CountAsync(ct);

            var items = await baseQuery.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize)
                .Select(v => new VaccinationRecordSimpleListItemDto
                {
                    RecordId = v.recordId,
                    VaccineName = v.vaccineDefinition.vaccineName,
                    Manufacturer = v.vaccineDefinition.manufacturer,
                    VaccinationDate = v.vaccinationDate,
                }).ToListAsync(ct);

            return new PagedResult<VaccinationRecordSimpleListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task AddVaccinationRecordAsync(CreateVaccinationRecordDto dto, CancellationToken ct)
        {
            // Validate citizen
            var citizen = await _dbContext.citizens.FindAsync(new object[] { dto.citizenId }, ct);
            if (citizen == null)
                throw new InvalidOperationException("Citizen not found.");

            // Validate vaccine definition
            var vaccineDef = await _dbContext.vaccineDefinitions.Include(v => v.vaccineDiseases).ThenInclude(vd => vd.disease)
                .FirstOrDefaultAsync(v => v.vaccineId == dto.vaccineId, ct);

            if (vaccineDef == null)
                throw new InvalidOperationException("Vaccine definition not found.");

            var record = new VaccinationRecord
            {
                citizenId = dto.citizenId,
                vaccineId = dto.vaccineId,
                vaccinationDate = dto.vaccinationDate,
                doseNumber = dto.doseNumber,
                nextVaccinationDate = dto.nextVaccinationDate,
                batch = dto.batch,
                sourceEntryId = dto.sourceEntryId,
                remarks = dto.remarks,
                creationDate = DateTime.UtcNow
            };

            await _dbContext.vaccinationRecords.AddAsync(record, ct);
            await _dbContext.SaveChangesAsync(ct);

            // Send notification
            var tokens = await _dbContext.citizenDeviceTokens
                          .Where(t => t.citizen == citizen)
                          .Select(t => new { t.deviceToken, t.language })
                          .ToListAsync();

            foreach (var token in tokens)
            {
                string title;
                string subtitle;
                if (token.language == "pl")
                {
                    title = "Dodano nowe szczepienie";
                    subtitle = $"Dodano szczepienie - {record.vaccineDefinition.vaccineName}";
                }
                else
                {
                    title = "New vaccination added";
                    subtitle = $"Added vaccination - {record.vaccineDefinition.vaccineName}";
                }
                await _notificationService.SendNotificationAsync(
                    token.deviceToken,
                    title,
                    subtitle
                );
                Console.WriteLine($"WYS£ANO POWIADOMIENIE - {token.language}");
            }
        }

        public async Task<VaccinationRecordEditDto?> GetVaccinationRecordByIdAsync(int recordId, CancellationToken ct)
        {
            var rec = await _dbContext.vaccinationRecords
                .AsNoTracking()
                .Include(x => x.citizen).Include(x => x.citizen.account)
                .Include(x => x.vaccineDefinition)
                .FirstOrDefaultAsync(x => x.recordId == recordId, ct);

            if (rec == null)
                return null;

            return new VaccinationRecordEditDto
            {
                recordId = rec.recordId,
                citizenId = rec.citizenId,
                name = rec.citizen.account.name,
                surname = rec.citizen.account.surname,
                pesel = rec.citizen.pesel,
                dateOfBirth = rec.citizen.account.dateOfBirth,
                vaccineId = rec.vaccineId,
                vaccineName = rec.vaccineDefinition.vaccineName,
                manufacturer = rec.vaccineDefinition.manufacturer,
                vaccinationDate = rec.vaccinationDate,
                nextVaccinationDate = rec.nextVaccinationDate,
                doseNumber = rec.doseNumber,
                batch = rec.batch,
                remarks = rec.remarks
            };
        }

        public async Task UpdateVaccinationRecordAsync(int recordId, UpdateVaccinationRecordDto dto, CancellationToken ct)
        {
            var rec = await _dbContext.vaccinationRecords.FirstOrDefaultAsync(x => x.recordId == recordId, ct);
            if (rec == null)
                throw new InvalidOperationException("Vaccination record not found.");

            if (dto.vaccinationDate == null)
                throw new ArgumentNullException("Vaccination date cannot be null.");

            rec.vaccinationDate = (DateTime)dto.vaccinationDate;
            rec.nextVaccinationDate = dto.nextVaccinationDate;
            rec.doseNumber = dto.doseNumber;
            rec.batch = dto.batch;
            rec.remarks = dto.remarks;

            _dbContext.vaccinationRecords.Update(rec);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task DeleteVaccinationRecordAsync(int recordId, CancellationToken ct)
        {
            var rec = await _dbContext.vaccinationRecords.FirstOrDefaultAsync(x => x.recordId == recordId, ct);
            if (rec == null)
                throw new InvalidOperationException("Vaccination record not found.");

            _dbContext.vaccinationRecords.Remove(rec);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task AddAsync(VaccinationRecord record, CancellationToken ct)
        {
            await _dbContext.vaccinationRecords.AddAsync(record, ct);
        }
    }
}