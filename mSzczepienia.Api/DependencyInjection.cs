using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using mSzczepienia.Api.Middlewares;
using mSzczepienia.Api.Services.Interfaces;
using mSzczepienia.Api.Services;
using System.Security.Claims;

namespace mSzczepienia.Api;

public static class DependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<LoggingMiddleware>();
        services.RegisterAutoMapper();
        services.RegisterFirebase(configuration);
        services.AddFirebaseAdmin(configuration);
        services.RegisterRepositories();
        services.RegisterServices();
        services.AddSingleton<IBlobService, BlobService>();

        return services;
    }

    public static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);
        return services;
    }

    public static IServiceCollection RegisterFirebase(this IServiceCollection services, IConfiguration configuration)
    {
        var projectId = configuration["Firebase:ProjectId"];

        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new Exception("No Firebase:ProjectId in configuration (appsettings.json).");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{projectId}";
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{projectId}",
                    ValidateAudience = true,
                    ValidAudience = projectId,
                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = "user_id"
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var identity = context.Principal?.Identity as ClaimsIdentity;
                        if (identity != null)
                        {
                            var roleClaimNames = new[] { "roles", "role", "admin" };

                            var roleClaims = context.Principal.Claims
                                .Where(c => roleClaimNames.Contains(c.Type, StringComparer.OrdinalIgnoreCase))
                                .ToList();

                            foreach (var rc in roleClaims)
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, rc.Value));
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }

    public static IServiceCollection AddFirebaseAdmin(this IServiceCollection services, IConfiguration config)
    {
        // Don't initialize Firebase in testing environment
        var environment = services.BuildServiceProvider().GetService<IHostEnvironment>();
        if (environment?.IsEnvironment("Testing") == true)
        {
            return services;
        }

        var jsonPath = config["Firebase:ServiceAccountKeyPath"];

        var serviceAccountCredential = CredentialFactory.FromFile<ServiceAccountCredential>(jsonPath);
        var googleCredential = serviceAccountCredential.ToGoogleCredential();

        // Only create if not already created
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = googleCredential
            });
        }

        return services;
    }

    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<Repositories.Interfaces.IFacilityRepository, Repositories.FacilityRepository>();
        services.AddScoped<Repositories.Interfaces.IStatsRepository, Repositories.StatsRepository>();
        services.AddScoped<Repositories.Interfaces.IVaccineRepository, Repositories.VaccineRepository>();
        services.AddScoped<Repositories.Interfaces.ICitizenRepository, Repositories.CitizenRepository>();
        services.AddScoped<Repositories.Interfaces.IAccountRepository, Repositories.AccountRepository>();
        services.AddScoped<Repositories.Interfaces.IAccountProfileRepository, Repositories.AccountProfileRepository>();
        services.AddScoped<Repositories.Interfaces.IPersonnelRepository, Repositories.PersonnelRepository>();
        services.AddScoped<Repositories.Interfaces.IVaccinationRecordRepository, Repositories.VaccinationRecordRepository>();
        services.AddScoped<Repositories.Interfaces.IOCRSubmissionRepository, Repositories.OCRSubmissionRepository>();

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<Services.Interfaces.IFacilityService, Services.FacilityService>();
        services.AddScoped<Services.Interfaces.IStatsService, Services.StatsService>();
        services.AddScoped<Services.Interfaces.IVaccineService, Services.VaccineService>();
        services.AddScoped<Services.Interfaces.IUserManagementService, Services.UserManagementService>();
        services.AddScoped<Services.Interfaces.ICitizenService, Services.CitizenService>();
        services.AddScoped<Services.Interfaces.IAccountService, Services.AccountService>();
        services.AddScoped<Services.Interfaces.IVaccinationRecordService, Services.VaccinationRecordService>();
        services.AddScoped<Services.Interfaces.IPersonnelService, Services.PersonnelService>();
        services.AddScoped<Services.Interfaces.IOCRSubmissionsService, Services.OCRSubmissionsService>();
        services.AddScoped<Services.Interfaces.IBlobService, Services.BlobService>();

        return services;
    }
}
