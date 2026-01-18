using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mSzczepienia.Api;
using mSzczepienia.Shared.Models;
using mSzczepienia.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace mSzczepienia.Tests.Integration.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private string _databaseName = "TestDatabase_" + Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove ALL DbContext-related services
            var dbContextDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Remove EntityFrameworkCore.SqlServer if it's registered
            var sqlServerDescriptor = services.FirstOrDefault(
                d => d.ServiceType.Namespace?.Contains("SqlServer") == true);
            if (sqlServerDescriptor != null)
            {
                services.Remove(sqlServerDescriptor);
            }

            // Add in-memory database for testing - USE SAME NAME FOR ALL REQUESTS
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Override JWT Bearer scheme for testing
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = null;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = GetSymmetricSecurityKey(),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = "user_id"
                };
            });

            // DO NOT build service provider - let the application build it during initialization
            // Instead, add this as a startup task
            services.AddSingleton<IHostedService>(sp =>
            {
                return new TestDataSeeder(sp);
            });
        });

        builder.UseEnvironment("Testing");
    }

    private class TestDataSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public TestDataSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
            SeedTestData(context);
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private static void SeedTestData(AppDbContext context)
    {
        // Add test facilities
        if (!context.medicalFacilities.Any())
        {
            context.medicalFacilities.AddRange(
                new MedicalFacility
                {
                    facilityId = 1,
                    facilityName = "Test Clinic 1",
                    street = "Main St",
                    city = "Warsaw",
                    postalCode = "00-001",
                    country = "Poland",
                    nip = "1234567890",
                    phoneNumber = "+48123456789",
                    creationDate = DateTime.UtcNow
                },
                new MedicalFacility
                {
                    facilityId = 2,
                    facilityName = "Test Clinic 2",
                    street = "Park Ave",
                    city = "Krakow",
                    postalCode = "31-001",
                    country = "Poland",
                    nip = "0987654321",
                    phoneNumber = "+48987654321",
                    creationDate = DateTime.UtcNow
                },
                new MedicalFacility
                {
                    facilityId = 3,
                    facilityName = "Test Hospital",
                    street = "Hospital Rd",
                    city = "Warsaw",
                    postalCode = "00-002",
                    country = "Poland",
                    nip = "1111111111",
                    phoneNumber = "+48111111111",
                    creationDate = DateTime.UtcNow
                }
            );
            context.SaveChanges();
        }

        // Add test diseases
        if (!context.diseases.Any())
        {
            context.diseases.AddRange(
                new Disease
                {
                    diseaseId = 1,
                    diseaseNameEn = "Influenza",
                    diseaseNamePl = "Grypa"
                },
                new Disease
                {
                    diseaseId = 2,
                    diseaseNameEn = "COVID-19",
                    diseaseNamePl = "COVID-19"
                },
                new Disease
                {
                    diseaseId = 3,
                    diseaseNameEn = "Measles",
                    diseaseNamePl = "Odra"
                }
            );
            context.SaveChanges();
        }

        // Add test vaccines
        if (!context.vaccineDefinitions.Any())
        {
            var vaccines = new List<VaccineDefinition>
            {
                new VaccineDefinition
                {
                    vaccineId = 1,
                    vaccineName = "Pfizer-BioNTech",
                    manufacturer = "Pfizer",
                    vaccineDescriptionEn = "mRNA COVID-19 vaccine",
                    vaccineDescriptionPl = "Szczepionka mRNA COVID-19",
                    shortDescriptionEn = "COVID-19 vaccine",
                    shortDescriptionPl = "Szczepionka COVID-19",
                    creationDate = DateTime.UtcNow
                },
                new VaccineDefinition
                {
                    vaccineId = 2,
                    vaccineName = "Moderna",
                    manufacturer = "Moderna",
                    vaccineDescriptionEn = "mRNA COVID-19 vaccine",
                    vaccineDescriptionPl = "Szczepionka mRNA COVID-19",
                    shortDescriptionEn = "COVID-19 vaccine",
                    shortDescriptionPl = "Szczepionka COVID-19",
                    creationDate = DateTime.UtcNow
                },
                new VaccineDefinition
                {
                    vaccineId = 3,
                    vaccineName = "Fluzone",
                    manufacturer = "Sanofi",
                    vaccineDescriptionEn = "Seasonal influenza vaccine",
                    vaccineDescriptionPl = "Szczepionka na grypę sezonową",
                    shortDescriptionEn = "Influenza vaccine",
                    shortDescriptionPl = "Szczepionka na grypę",
                    creationDate = DateTime.UtcNow
                },
                new VaccineDefinition
                {
                    vaccineId = 4,
                    vaccineName = "M-M-R II",
                    manufacturer = "Merck",
                    vaccineDescriptionEn = "Measles, mumps, rubella vaccine",
                    vaccineDescriptionPl = "Szczepionka na odrę, świnkę, różyczkę",
                    shortDescriptionEn = "MMR vaccine",
                    shortDescriptionPl = "Szczepionka MMR",
                    creationDate = DateTime.UtcNow
                }
            };

            context.vaccineDefinitions.AddRange(vaccines);
            context.SaveChanges();

            // Add vaccine-disease relationships
            var vaccineDiseases = new List<VaccineDisease>
            {
                new VaccineDisease { vaccineId = 1, diseaseId = 2 },
                new VaccineDisease { vaccineId = 2, diseaseId = 2 },
                new VaccineDisease { vaccineId = 3, diseaseId = 1 },
                new VaccineDisease { vaccineId = 4, diseaseId = 3 }
            };

            context.vaccineDiseases.AddRange(vaccineDiseases);
            context.SaveChanges();
        }

        // Add test accounts for citizens
        List<Account> citizenAccounts = new();
        if (!context.accounts.Any(a => a.firebaseUid == "test-citizen-uid-1"))
        {
            citizenAccounts = new List<Account>
            {
                new Account
                {
                    accountId = Guid.NewGuid(),
                    firebaseUid = "test-citizen-uid-1",
                    email = "citizen1@test.com",
                    role = Role.Citizen,
                    name = "John",
                    surname = "Smith",
                    creationDate = DateTime.UtcNow,
                    dateOfBirth = new DateTime(1980, 3, 10)
                },
                new Account
                {
                    accountId = Guid.NewGuid(),
                    firebaseUid = "test-citizen-uid-2",
                    email = "citizen2@test.com",
                    role = Role.Citizen,
                    name = "Anna",
                    surname = "Johnson",
                    creationDate = DateTime.UtcNow,
                    dateOfBirth = new DateTime(1988, 7, 25)
                }
            };
            context.accounts.AddRange(citizenAccounts);
            context.SaveChanges();
        }
        else
        {
            citizenAccounts = context.accounts
                .Where(a => a.firebaseUid == "test-citizen-uid-1" || a.firebaseUid == "test-citizen-uid-2")
                .ToList();
        }

        // Add test OCR submissions for stats
        if (!context.ocrSubmissions.Any())
        {
            // Create test citizens first
            var citizens = new List<Citizen>
            {
                new Citizen
                {
                    citizenId = 1,
                    accountId = citizenAccounts.First(a => a.firebaseUid == "test-citizen-uid-1").accountId,
                    account = citizenAccounts.First(a => a.firebaseUid == "test-citizen-uid-1"),
                    pesel = "12345678901",
                    facilityId = 1,
                    submissionStatus = SubmissionStatus.Pending
                },
                new Citizen
                {
                    citizenId = 2,
                    accountId = citizenAccounts.First(a => a.firebaseUid == "test-citizen-uid-2").accountId,
                    account = citizenAccounts.First(a => a.firebaseUid == "test-citizen-uid-2"),
                    pesel = "98765432101",
                    facilityId = 2,
                    submissionStatus = SubmissionStatus.Pending
                },
                new Citizen
                {
                    citizenId = 3,
                    accountId = Guid.NewGuid(),
                    pesel = "55555555555",
                    facilityId = 3,
                    submissionStatus = SubmissionStatus.Pending
                }
            };

            context.citizens.AddRange(citizens);
            context.SaveChanges();

            // Create OCR submissions with different statuses
            var ocrSubmissions = new List<OcrSubmission>
            {
                new OcrSubmission
                {
                    submissionId = 1,
                    citizenId = 1,
                    photoUrl = "https://example.com/photo1.jpg",
                    status = OcrSubmissionStatus.Accepted,
                    submissionDate = DateTime.UtcNow.AddDays(-5),
                    verificationDate = DateTime.UtcNow.AddDays(-4)
                },
                new OcrSubmission
                {
                    submissionId = 2,
                    citizenId = 2,
                    photoUrl = "https://example.com/photo2.jpg",
                    status = OcrSubmissionStatus.Accepted,
                    submissionDate = DateTime.UtcNow.AddDays(-3),
                    verificationDate = DateTime.UtcNow.AddDays(-2)
                },
                new OcrSubmission
                {
                    submissionId = 3,
                    citizenId = 1,
                    photoUrl = "https://example.com/photo3.jpg",
                    status = OcrSubmissionStatus.Pending,
                    submissionDate = DateTime.UtcNow.AddDays(-1)
                },
                new OcrSubmission
                {
                    submissionId = 4,
                    citizenId = 2,
                    photoUrl = "https://example.com/photo4.jpg",
                    status = OcrSubmissionStatus.Pending,
                    submissionDate = DateTime.UtcNow
                },
                new OcrSubmission
                {
                    submissionId = 5,
                    citizenId = 3,
                    photoUrl = "https://example.com/photo5.jpg",
                    status = OcrSubmissionStatus.Rejected,
                    submissionDate = DateTime.UtcNow.AddDays(-2),
                    verifierNotes = "Poor quality image",
                    verificationDate = DateTime.UtcNow.AddDays(-1)
                }
            };

            context.ocrSubmissions.AddRange(ocrSubmissions);
            context.SaveChanges();

            // Create Accounts for personnel and admins
            var accounts = new List<Account>
            {
                new Account
                {
                    accountId = Guid.NewGuid(),
                    firebaseUid = "test-admin-uid-1",
                    email = "admin1@test.com",
                    role = Role.Admin,
                    name = "AdminFirst",
                    surname = "AdminOne",
                    creationDate = DateTime.UtcNow,
                    dateOfBirth = new DateTime(1980, 2, 10)
                },
                new Account
                {
                    accountId = Guid.NewGuid(),
                    firebaseUid = "test-admin-uid-2",
                    email = "admin2@test.com",
                    role = Role.Admin,
                    name = "AdminSecond",
                    surname = "AdminTwo",
                    creationDate = DateTime.UtcNow,
                    dateOfBirth = new DateTime(1982, 6, 15)
                },
                new Account
                {
                    accountId = Guid.NewGuid(),
                    firebaseUid = "test-admin-uid-3",
                    email = "admin3@test.com",
                    role = Role.Admin,
                    name = "AdminThird",
                    surname = "AdminThree",
                    creationDate = DateTime.UtcNow,
                    dateOfBirth = new DateTime(1984, 12, 20)
                },
                new Account
                {
                    accountId = Guid.NewGuid(),
                    firebaseUid = "test-worker-uid-1",
                    email = "worker1@test.com",
                    role = Role.HealthWorker,
                    name = "Jan",
                    surname = "Kowalski",
                    creationDate = DateTime.UtcNow,
                    dateOfBirth = new DateTime(1985, 5, 15)
                },
                new Account
                {
                    accountId = Guid.NewGuid(),
                    firebaseUid = "test-worker-uid-2",
                    email = "worker2@test.com",
                    role = Role.HealthWorker,
                    name = "Maria",
                    surname = "Nowak",
                    creationDate = DateTime.UtcNow,
                    dateOfBirth = new DateTime(1990, 8, 20)
                }
            };

            context.accounts.AddRange(accounts);
            context.SaveChanges();

            // Create Personnel linked to Accounts and Facilities
            var personnel = new List<Personnel>
            {
                new Personnel
                {
                    personnelId = 1,
                    accountId = accounts[3].accountId,  // Using worker-uid-1 (index 3)
                    account = accounts[3],
                    facilityId = 1,
                    pwzNumber = "PWZ123456",
                    position = Position.Nurse
                },
                new Personnel
                {
                    personnelId = 2,
                    accountId = accounts[4].accountId,  // Using worker-uid-2 (index 4)
                    account = accounts[4],
                    facilityId = 2,
                    pwzNumber = "PWZ654321",
                    position = Position.Doctor
                }
            };

            context.personnel.AddRange(personnel);
            context.SaveChanges();

            // Add vaccination records for testing
            var vaccinationRecords = new List<VaccinationRecord>
            {
                new VaccinationRecord
                {
                    recordId = 1,
                    citizenId = 1,
                    vaccineId = 1,
                    vaccinationDate = DateTime.UtcNow.AddDays(-30),
                    doseNumber = "1",
                    batch = "BATCH001",
                    nextVaccinationDate = DateTime.UtcNow.AddDays(21),
                    remarks = "First dose",
                    creationDate = DateTime.UtcNow.AddDays(-30)
                },
                new VaccinationRecord
                {
                    recordId = 2,
                    citizenId = 1,
                    vaccineId = 1,
                    vaccinationDate = DateTime.UtcNow.AddDays(-10),
                    doseNumber = "2",
                    batch = "BATCH002",
                    nextVaccinationDate = null,
                    remarks = "Second dose",
                    creationDate = DateTime.UtcNow.AddDays(-10)
                },
                new VaccinationRecord
                {
                    recordId = 3,
                    citizenId = 1,
                    vaccineId = 3,
                    vaccinationDate = DateTime.UtcNow.AddDays(-5),
                    doseNumber = "1",
                    batch = "BATCH003",
                    nextVaccinationDate = null,
                    remarks = "Influenza vaccine",
                    creationDate = DateTime.UtcNow.AddDays(-5)
                },
                new VaccinationRecord
                {
                    recordId = 4,
                    citizenId = 2,
                    vaccineId = 2,
                    vaccinationDate = DateTime.UtcNow.AddDays(-20),
                    doseNumber = "1",
                    batch = "BATCH004",
                    nextVaccinationDate = DateTime.UtcNow.AddDays(21),
                    remarks = "First dose Moderna",
                    creationDate = DateTime.UtcNow.AddDays(-20)
                },
                new VaccinationRecord
                {
                    recordId = 5,
                    citizenId = 2,
                    vaccineId = 4,
                    vaccinationDate = DateTime.UtcNow.AddDays(-15),
                    doseNumber = "1",
                    batch = "BATCH005",
                    remarks = "MMR vaccine",
                    creationDate = DateTime.UtcNow.AddDays(-15)
                }
            };

            context.vaccinationRecords.AddRange(vaccinationRecords);
            context.SaveChanges();
        }
    }

    public HttpClient CreateClientWithAuth(string? role = null, string? firebaseUid = null)
    {
        var client = CreateClient();
        var token = GenerateTestJwtToken(role, firebaseUid);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public HttpClient CreateClientWithoutAuth()
    {
        return CreateClient();
    }

    public string GenerateTestJwtToken(string? role = null, string? firebaseUid = null)
    {
        var key = GetSymmetricSecurityKey();
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("user_id", firebaseUid ?? "test-user-id"),
            new Claim(ClaimTypes.NameIdentifier, firebaseUid ?? "test-user-id"),
            new Claim(ClaimTypes.Name, "Test User")
        };

        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-a-very-long-secret-key-for-testing-purposes-only-12345"));
    }
}
