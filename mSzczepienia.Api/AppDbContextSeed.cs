using Bogus;
using Bogus.DataSets;
using Bogus.Extensions.Poland;
using CsvHelper;
using mSzczepienia.Api.Data;
using mSzczepienia.Shared.Enums;
using mSzczepienia.Shared.Models;
using System.Globalization;
namespace mSzczepienia.Api
{
    public class AppDbContextSeed
    {
        private readonly AppDbContext _dbContext;

        public AppDbContextSeed(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (!_dbContext.Database.CanConnect()) return;

            // Medical Facilities
            if (!_dbContext.medicalFacilities.Any())
            {
                using (var reader = new StreamReader("Data/Csv/seeds_facilities.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var facilitiesRecords = csv.GetRecords<FacilitiesCsvModel>().ToList();
                    var entities = facilitiesRecords.Select(x => new MedicalFacility
                    {
                        facilityName = x.facilityName,
                        street = x.street,
                        postalCode = x.postalCode,
                        city = x.city,
                        country = x.country,
                        nip = x.nip,
                        phoneNumber = x.phoneNumber,
                        creationDate = x.creationDate
                    });

                    _dbContext.medicalFacilities.AddRange(entities);
                    _dbContext.SaveChanges();
                }
            }

            // Accounts, Personnel and Citizens
            if (!_dbContext.accounts.Any())
            {
                var facilities = _dbContext.medicalFacilities.ToList();

                // Personnel
                var personnelAccounts = GenerateAccounts(6000, Role.HealthWorker);
                _dbContext.accounts.AddRange(personnelAccounts);
                _dbContext.SaveChanges();

                var personnel = GeneratePersonnels(personnelAccounts, facilities);
                _dbContext.personnel.AddRange(personnel);

                var testPersonnelAccount = new Account
                {
                    email = "piotr.istok@gmail.com",
                    firebaseUid = "OwzOPEW7PNX2e1jsfM8KLdiMieU2",
                    role = Role.HealthWorker,
                    name = "Piotr",
                    surname = "Iśtok",
                    creationDate = DateTime.Now,
                    dateOfBirth = new DateTime(2003, 11, 18)
                };

                _dbContext.accounts.Add(testPersonnelAccount);
                _dbContext.SaveChanges();

                var testPersonnel = new Personnel
                {
                    accountId = testPersonnelAccount.accountId,
                    facilityId = facilities.First().facilityId,
                    pwzNumber = "2654712",
                    position = Position.Doctor,
                };

                _dbContext.personnel.Add(testPersonnel);
                _dbContext.SaveChanges();

                // Citizens
                var citizenAccounts = GenerateAccounts(50000, Role.Citizen).ToList();
                var testCitizenAccount = new Account
                {
                    email = "piotrjac23@gmail.com",
                    firebaseUid = "UwJYlRuRKHfz6U1C3hFudMYxIOU2",
                    role = Role.Citizen,
                    name = "Piotr",
                    surname = "Jacak",
                    creationDate = DateTime.Now,
                    dateOfBirth = new DateTime(2003, 02, 06)
                };
                citizenAccounts.Add(testCitizenAccount);
                _dbContext.accounts.AddRange(citizenAccounts);
                _dbContext.SaveChanges();

                var citizens = GenerateCitizens(citizenAccounts, facilities);
                _dbContext.citizens.AddRange(citizens);

                // Admins
                var adminAccounts = GenerateAccounts(5, Role.Admin);
                _dbContext.accounts.AddRange(adminAccounts);

                var testAdminAccount = new Account
                {
                    email = "1stok13piotr@gmail.com",
                    firebaseUid = "FPXYH9A1WYOZu9VRkPt789lN99x2",
                    role = Role.Admin,
                    name = "Piotr",
                    surname = "Iśtok",
                    creationDate = DateTime.Now,
                    dateOfBirth = new DateTime(2003, 11, 18)
                };

                _dbContext.accounts.Add(testAdminAccount);
                _dbContext.SaveChanges();

                _dbContext.SaveChanges();
            }

            // OCR Submissions
            if (!_dbContext.ocrSubmissions.Any())
            {
                var existingCitizens = _dbContext.citizens.ToList();
                var existingVerifiers = _dbContext.personnel.ToList();

                var submissions = GenerateOcrSubmissions(existingCitizens, existingVerifiers);
                _dbContext.ocrSubmissions.AddRange(submissions);
                _dbContext.SaveChanges();
            }

            // Diseases
            if (!_dbContext.diseases.Any())
            {
                using (var reader = new StreamReader("Data/Csv/seeds_diseases.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var diseaseRecords = csv.GetRecords<DiseaseCsvModel>().ToList();
                    var entities = diseaseRecords.Select(x => new Disease { diseaseNameEn = x.diseaseNameEn, diseaseNamePl = x.diseaseNamePl });

                    _dbContext.diseases.AddRange(entities);
                    _dbContext.SaveChanges();
                }
            }

            // Vaccines
            if (!_dbContext.vaccineDefinitions.Any())
            {
                using (var reader = new StreamReader("Data/Csv/seeds_vaccines.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var vaccineRecords = csv.GetRecords<VaccineCsvModel>().ToList();
                    var entities = vaccineRecords.Select(x => new VaccineDefinition
                    {
                        vaccineName = x.vaccineName,
                        manufacturer = x.manufacturer,
                        vaccineDescriptionEn = x.vaccineDescriptionEn,
                        vaccineDescriptionPl = x.vaccineDescriptionPl,
                        shortDescriptionEn = x.shortDescriptionEn,
                        shortDescriptionPl = x.shortDescriptionPl,
                        creationDate = x.creationDate
                    });

                    _dbContext.vaccineDefinitions.AddRange(entities);
                    _dbContext.SaveChanges();
                }
            }

            // Vaccine for disease
            if (!_dbContext.vaccineDiseases.Any())
            {
                var vaccinesMap = _dbContext.vaccineDefinitions
                    .ToDictionary(v => v.vaccineName, v => v.vaccineId);

                var diseasesMap = _dbContext.diseases
                    .ToDictionary(d => d.diseaseNameEn, d => d.diseaseId);


                var relations = new List<VaccineDisease>();
                using (var reader = new StreamReader("Data/Csv/seeds_relations.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<VaccineDiseaseCsvModel>();

                    foreach (var row in records)
                    {
                        if (vaccinesMap.TryGetValue(row.vaccineName, out int vId) &&
                            diseasesMap.TryGetValue(row.diseaseName, out int dId))
                        {
                            relations.Add(new VaccineDisease
                            {
                                vaccineId = vId,
                                diseaseId = dId
                            });
                        }
                    }
                }

                _dbContext.vaccineDiseases.AddRange(relations);
                _dbContext.SaveChanges();
            }

            // Vaccination Entries
            if (!_dbContext.vaccinationEntries.Any())
            {
                var existingOcrSubmissions = _dbContext.ocrSubmissions.ToList();
                var existingVaccines = _dbContext.vaccineDefinitions.ToList();
                var entries = GenerateVaccinationEntries(existingOcrSubmissions, existingVaccines);
                _dbContext.vaccinationEntries.AddRange(entries);
                _dbContext.SaveChanges();
            }

            // Vaccination Records
            if (!_dbContext.vaccinationRecords.Any())
            {
                var existingVaccinationEntries = _dbContext.vaccinationEntries.ToList();
                var records = GenerateVaccinationRecords(existingVaccinationEntries);
                _dbContext.vaccinationRecords.AddRange(records);
                _dbContext.SaveChanges();
            }

        }


        // 1. GENERATING ACCOUNTS (INDEPENDENTLY FOR EACH ROLE)
        private IEnumerable<Account> GenerateAccounts(int count, Role roleName)
        {
            var faker = new Faker<Account>("pl")
                .RuleFor(a => a.email, (f, u) => f.Internet.Email(u.name, u.surname))
                .RuleFor(a => a.firebaseUid, f => f.Random.AlphaNumeric(28))
                .RuleFor(a => a.role, f => roleName)
                .RuleFor(a => a.name, f => f.Name.FirstName())
                .RuleFor(a => a.surname, f => f.Name.LastName())
                .RuleFor(a => a.creationDate, f => f.Date.Past(2))
                .RuleFor(a => a.dateOfBirth, f => f.Date.Past(60, DateTime.Now.AddYears(-18))); // Adults

            return faker.Generate(count);
        }

        // 2. GENERATING PERSONNEL 
        private IEnumerable<Personnel> GeneratePersonnels(
            IEnumerable<Account> accounts,
            IEnumerable<MedicalFacility> facilities)
        {
            var facilitiesList = facilities.ToList();
            var personnels = new List<Personnel>();
            var faker = new Faker("pl");

            // Assuming accounts are only with role Personnel
            foreach (var account in accounts)
            {
                var personnel = new Personnel
                {
                    accountId = account.accountId,
                    facilityId = faker.PickRandom(facilitiesList).facilityId, // Random facility
                    pwzNumber = faker.Random.ReplaceNumbers("#######"),
                    position = faker.PickRandom<Position>()
                };
                personnels.Add(personnel);
            }

            return personnels;
        }

        // 3. GENERATING CITIZENS
        private IEnumerable<Citizen> GenerateCitizens(
            IEnumerable<Account> accounts,
            IEnumerable<MedicalFacility> facilities)
        {
            var facilitiesList = facilities.ToList();
            var citizens = new List<Citizen>();

            // Assuming accounts are only with role Citizen
            foreach (var account in accounts)
            {
                var faker = new Faker("pl");

                var citizen = new Citizen
                {
                    accountId = account.accountId,
                    facilityId = faker.PickRandom(facilitiesList).facilityId,
                    pesel = faker.Person.Pesel(),
                    submissionStatus = faker.PickRandom<SubmissionStatus>()
                };
                citizens.Add(citizen);
            }

            return citizens;
        }

        // 4. GENERATING OCR SUBMISSIONS
        private IEnumerable<OcrSubmission> GenerateOcrSubmissions(
            IEnumerable<Citizen> citizens,
            IEnumerable<Personnel> verifiers)
        {
            var citizensList = citizens.ToList();
            var verifiersList = verifiers.ToList();

            var faker = new Faker("pl");

            return citizensList.Select(citizen =>
            {
                var submission = new OcrSubmission
                {
                    citizenId = citizen.citizenId,
                    photoUrl = faker.Internet.Url() + "/scan.jpg",
                    status = faker.PickRandom<OcrSubmissionStatus>(),
                    submissionDate = faker.Date.Past(1)
                };

                if (submission.status != OcrSubmissionStatus.Pending)
                {
                    submission.verifierId = faker.PickRandom(verifiersList).personnelId;
                    submission.verificationDate = submission.submissionDate.AddHours(faker.Random.Int(1, 48));
                    submission.verifierNotes = faker.Lorem.Sentence();
                }
                else
                {
                    submission.verifierId = null;
                    submission.verificationDate = null;
                    submission.verifierNotes = null;
                }

                return submission;
            });
        }

        // 5. GENERATING VACCINATION ENTRIES
        private IEnumerable<VaccinationEntry> GenerateVaccinationEntries(
            IEnumerable<OcrSubmission> ocrSubmissions,
            IEnumerable<VaccineDefinition> vaccineDefinitions)
        {
            var ocrSubmissionList = ocrSubmissions.ToList();
            var vaccineDefinitionsList = vaccineDefinitions.ToList();

            var vaccinationEntries = new List<VaccinationEntry>();

            var faker = new Faker("pl");
            foreach (var ocrSubmission in ocrSubmissionList)
            {
                // Generate 10 different vaccines
                var randomVaccines = faker.PickRandom(vaccineDefinitionsList, 10).ToList();

                if (ocrSubmission.status == OcrSubmissionStatus.Rejected)
                {
                    continue;
                }

                foreach (var vaccine in randomVaccines)
                {
                    var randomDate = faker.Date.Past(1);

                    // Max 3 doses for each vaccine
                    int doses = faker.Random.Int(1, 3);
                    for (int i = 0; i < doses; i++)
                    {
                        VaccinationEntryStatus vaccinationEntryStatus;
                        if (ocrSubmission.status == OcrSubmissionStatus.Accepted)
                        {
                            vaccinationEntryStatus = faker.PickRandom<VaccinationEntryStatus>(new[] {
                                VaccinationEntryStatus.Processed,
                                VaccinationEntryStatus.CreatedByUser,
                                VaccinationEntryStatus.Deleted
                            });
                        }
                        else
                        {
                            vaccinationEntryStatus = VaccinationEntryStatus.WaitingForReview;
                        }
                        randomDate = randomDate.AddDays(faker.Random.Int(20, 60));
                        var randomBatch = faker.Random.AlphaNumeric(8).ToUpper();

                        var entry = new VaccinationEntry
                        {
                            ocrSubmissionId = ocrSubmission.submissionId,
                            vaccineNameOcr = vaccine.vaccineName,
                            dateOcr = randomDate.ToString("yyyy-MM-dd"),
                            batchOcr = randomBatch,
                            vaccineId = vaccine.vaccineId,
                            date = DateTime.Parse(randomDate.ToString("yyyy-MM-dd")),
                            batch = randomBatch,
                            status = vaccinationEntryStatus
                        };
                        vaccinationEntries.Add(entry);
                    }
                }
            }

            return vaccinationEntries;
        }

        // 6. GENERATING VACCINATION RECORDS
        private IEnumerable<VaccinationRecord> GenerateVaccinationRecords(IEnumerable<VaccinationEntry> vaccinationEntries)
        {
            var vaccinationEntriesList = vaccinationEntries.ToList();
            var vaccinationRecords = new List<VaccinationRecord>();
            var faker = new Faker("pl");

            var groupedEntries = vaccinationEntriesList
                .Where(x => x.ocrSubmission != null && x.vaccineId != null && x.date != null &&
                       x.status != VaccinationEntryStatus.Deleted && x.status != VaccinationEntryStatus.WaitingForReview)
                .GroupBy(x => new { x.ocrSubmission.citizenId, x.vaccineId });

            foreach (var group in groupedEntries)
            {
                var sortedDoses = group.OrderBy(x => x.date).ToList();
                for (int i = 0; i < sortedDoses.Count; i++)
                {
                    var entry = sortedDoses[i];
                    var calculatedDoseNumber = (i + 1).ToString();
                    var randomBatch = faker.Random.AlphaNumeric(10).ToUpper();

                    var record = new VaccinationRecord
                    {
                        citizenId = entry.ocrSubmission.citizenId,
                        vaccineId = (int)entry.vaccineId!,
                        batch = randomBatch,
                        vaccinationDate = (DateTime)entry.date!,
                        doseNumber = calculatedDoseNumber,
                        sourceEntryId = entry.vaccinationEntryId,
                        creationDate = DateTime.Now,
                    };
                    vaccinationRecords.Add(record);
                }
            }

            return vaccinationRecords;
        }
    }
}

