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
                    vaccineDescription = "mRNA COVID-19 vaccine",
                    creationDate = DateTime.UtcNow
                },
                new VaccineDefinition
                {
                    vaccineId = 2,
                    vaccineName = "Moderna",
                    manufacturer = "Moderna",
                    vaccineDescription = "mRNA COVID-19 vaccine",
                    creationDate = DateTime.UtcNow
                },
                new VaccineDefinition
                {
                    vaccineId = 3,
                    vaccineName = "Fluzone",
                    manufacturer = "Sanofi",
                    vaccineDescription = "Seasonal influenza vaccine",
                    creationDate = DateTime.UtcNow
                },
                new VaccineDefinition
                {
                    vaccineId = 4,
                    vaccineName = "M-M-R II",
                    manufacturer = "Merck",
                    vaccineDescription = "Measles, mumps, rubella vaccine",
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

        // Add test OCR submissions for stats
        if (!context.ocrSubmissions.Any())
        {
            // Create test citizens first
            var citizens = new List<Citizen>
            {
                new Citizen
                {
                    citizenId = 1,
                    accountId = Guid.NewGuid(),
                    pesel = "12345678901",
                    facilityId = 1,
                    submissionStatus = SubmissionStatus.Pending
                },
                new Citizen
                {
                    citizenId = 2,
                    accountId = Guid.NewGuid(),
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
        }
    }

    public HttpClient CreateClientWithAuth(string? role = null)
    {
        var client = CreateClient();
        var token = GenerateTestJwtToken(role);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public HttpClient CreateClientWithoutAuth()
    {
        return CreateClient();
    }

    public string GenerateTestJwtToken(string? role = null)
    {
        var key = GetSymmetricSecurityKey();
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("user_id", "test-user-id"),
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
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
