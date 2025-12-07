using Bogus;
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
                var facilities = GenerateMedicalFacilities(10);
                _dbContext.medicalFacilities.AddRange(facilities);
                _dbContext.SaveChanges();
            }

            // Accounts, Personnel and Citizens
            if (!_dbContext.accounts.Any())
            {
                var facilities = _dbContext.medicalFacilities.ToList();

                // Personnel
                var personnelAccounts = GenerateAccounts(20, Role.HealthWorker);
                _dbContext.accounts.AddRange(personnelAccounts);
                _dbContext.SaveChanges();

                var personnel = GeneratePersonnels(personnelAccounts, facilities);
                _dbContext.personnel.AddRange(personnel);


                // Citizens
                var citizenAccounts = GenerateAccounts(50, Role.Citizen).ToList();
                var testCitizenAccount = new Account
                {
                    email = "piotrjac23@gmail.com",
                    firebaseUid = "UwJYlRuRKHfz6U1C3hFudMYxIOU2",
                    role = Role.Citizen,
                    name = "Mateusz",
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



                _dbContext.SaveChanges();
            }

            // OCR Submissions
            if (!_dbContext.ocrSubmissions.Any())
            {
                var existingCitizens = _dbContext.citizens.ToList();
                var existingVerifiers = _dbContext.personnel.ToList();

                var submissions = GenerateOcrSubmissions(50, existingCitizens, existingVerifiers);
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
                        vaccineDescription = x.vaccineDescription,
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



        // 1. GENERATING MEDICAL FACILITIES
        private IEnumerable<MedicalFacility> GenerateMedicalFacilities(int count)
        {
            var faker = new Faker<MedicalFacility>("pl")
                .RuleFor(m => m.facilityName, f => "NFZ " + f.Company.CompanyName())
                .RuleFor(m => m.address, f => f.Address.FullAddress())
                .RuleFor(m => m.nip, f => f.Random.ReplaceNumbers("##########"))
                .RuleFor(m => m.creationDate, f => f.Date.Past(5));

            return faker.Generate(count);
        }

        // 2. GENERATING ACCOUNTS (INDEPENDENTLY FOR EACH ROLE)
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

        // 3. GENERATING PERSONNEL 
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

        // 4. GENERATING CITIZENS
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
                    peselHash = faker.Person.Pesel(),
                    submissionStatus = faker.PickRandom<SubmissionStatus>()
                };
                citizens.Add(citizen);
            }

            return citizens;
        }

        // 5. GENERATING OCR SUBMISSIONS
        private IEnumerable<OcrSubmission> GenerateOcrSubmissions(
            int count,
            IEnumerable<Citizen> citizens,
            IEnumerable<Personnel> verifiers)
        {
            var citizensList = citizens.ToList();
            var verifiersList = verifiers.ToList();

            var faker = new Faker<OcrSubmission>("pl")
                .RuleFor(s => s.citizenId, f => f.PickRandom(citizensList).citizenId)
                .RuleFor(s => s.photoUrl, f => f.Internet.Url() + "/scan.jpg")
                .RuleFor(s => s.status, f => f.PickRandom<OcrSubmissionStatus>())
                .RuleFor(s => s.submissionDate, f => f.Date.Past(1));

            // Logic for records which have been verified
            return faker.Generate(count).Select(s =>
            {
                if (s.status != OcrSubmissionStatus.Pending)
                {
                    s.verifierId = new Faker().PickRandom(verifiersList).personnelId;
                    s.verificationDate = s.submissionDate.AddHours(new Faker().Random.Int(1, 48));
                    s.verifierNotes = new Faker("pl").Lorem.Sentence();
                }
                else
                {
                    s.verifierId = null;
                    s.verificationDate = null;
                    s.verifierNotes = null;
                }
                return s;
            });
        }

        // 6. GENERATING VACCINATION ENTRIES
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
                for (int i = 0; i < 5; i++) 
                {
                    var randomVaccine = faker.PickRandom(vaccineDefinitionsList);
                    var randomBatch = faker.Random.AlphaNumeric(10).ToUpper();
                    var randomDate = faker.Date.Past(1).ToString("yyyy-MM-dd");

                    var entry = new VaccinationEntry
                    {
                        ocrSubmissionId = ocrSubmission.submissionId,
                        batchOcr = randomBatch,
                        vaccineNameOcr = randomVaccine.vaccineName,
                        dateOcr = randomDate,
                        vaccineId = randomVaccine.vaccineId,
                        batch = randomBatch,
                        date = DateTime.Parse(randomDate),
                    };
                    vaccinationEntries.Add(entry);
                }
            }

            return vaccinationEntries;
        }

        // 7. GENERATING VACCINATION RECORDS
        private IEnumerable<VaccinationRecord> GenerateVaccinationRecords(IEnumerable<VaccinationEntry> vaccinationEntries)
        {
            var vaccinationEntriesList = vaccinationEntries.ToList();

            var vaccinationRecords = new List<VaccinationRecord>();

            var faker = new Faker("pl");
            foreach (var vaccinationEntry in vaccinationEntriesList)
            {
                if (vaccinationEntry.vaccineId != null &&
                    vaccinationEntry.batch != null &&
                    vaccinationEntry.date != null)
                {
                    var record = new VaccinationRecord
                    {

                        citizenId = vaccinationEntry.ocrSubmission.citizenId,
                        vaccineId = (int)vaccinationEntry.vaccineId,
                        batch = vaccinationEntry.batch,
                        vaccinationDate = (DateTime)vaccinationEntry.date,
                        doseNumber = faker.Random.Int(1, 3).ToString(),
                        sourceEntryId = vaccinationEntry.vaccinationEntryId,
                        creationDate = DateTime.Now,
                    };
                    vaccinationRecords.Add(record);
                }

            }
            return vaccinationRecords;
        }
    }
}

