using mSzczepienia.Shared.DTO.Queries;
using mSzczepienia.Shared.DTO.VaccinationRecords;
using mSzczepienia.Shared.DTO;
using mSzczepienia.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var baseQuery = _dbContext.vaccinationRecords
                .AsNoTracking()
                .Where(x => x.citizen.account.firebaseUid == firebaseUid);

            var groupedQuery = baseQuery
                .GroupBy(x => new { x.vaccineDefinition.vaccineId, x.vaccineDefinition.vaccineName, x.vaccineDefinition.manufacturer })
                .Select(g => new VaccinationRecordOverviewDto
                {
                    id = g.Key.vaccineId,
                    vaccineName = g.Key.vaccineName,
                    manufacturer = g.Key.manufacturer,
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

        public async Task<PagedResult<VaccinationRecordOverviewDto>> GetVaccinationRecordOverviewByCitizenIdAsync(GetVaccinationRecordsByCitizenIdQuery query, CancellationToken ct)
        {
            var baseQuery = _dbContext.vaccinationRecords.AsNoTracking().Where(x => x.citizen.citizenId == query.CitizenId);

            var groupedQuery = baseQuery.GroupBy(x => new { x.vaccineDefinition.vaccineId, x.vaccineDefinition.vaccineName, x.vaccineDefinition.manufacturer })
                .Select(g => new VaccinationRecordOverviewDto
                {
                    id = g.Key.vaccineId,
                    vaccineName = g.Key.vaccineName,
                    manufacturer = g.Key.manufacturer,
                    vaccinationDate = g.Max(x => x.vaccinationDate),
                    dosesNumber = g.Count()
                });

            var sortField = query.SortingField?.ToLower() ?? "vaccinationdate";
            var isAsc = query.SortingDirection?.ToLower() == "asc";

            groupedQuery = sortField switch
            {
                "vaccinename" => isAsc ? groupedQuery.OrderBy(x => x.vaccineName) : groupedQuery.OrderByDescending(x => x.vaccineName),

                _ => isAsc ? groupedQuery.OrderBy(x => x.vaccinationDate) : groupedQuery.OrderByDescending(x => x.vaccinationDate)
            };

            var totalCount = await groupedQuery.CountAsync(ct);

            var items = await groupedQuery.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);

            return new PagedResult<VaccinationRecordOverviewDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}