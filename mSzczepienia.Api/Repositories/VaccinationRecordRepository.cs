using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api.Repositories.Interfaces;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api.Repositories
{
    public class VaccinationRecordRepository : IVaccinationRecordRepository
    {
        private readonly AppDbContext _dbContext;

        public VaccinationRecordRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public async Task AddAsync(VaccinationRecord record, CancellationToken ct)
        {
            await _dbContext.vaccinationRecords.AddAsync(record, ct);
        }

        public async Task<VaccinationRecord?> GetById(int id, CancellationToken ct)
        {
            return await _dbContext.vaccinationRecords.FirstOrDefaultAsync(x => x.recordId == id, ct);
        }

        public async Task<VaccinationRecord?> GetByIdNoTrackingAsync(int id, CancellationToken ct)
        {
            return await _dbContext.vaccinationRecords.AsNoTracking().FirstOrDefaultAsync(x => x.recordId == id, ct);
        }

        public void Update(VaccinationRecord record)
        {
            _dbContext.vaccinationRecords.Update(record);
        }

        public void Delete(VaccinationRecord record)
        {
            _dbContext.vaccinationRecords.Remove(record);
        }
    }
}