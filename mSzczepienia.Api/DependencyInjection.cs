using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using mSzczepienia.Api.Middlewares;
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
        var jsonPath = config["Firebase:ServiceAccountKeyPath"];

        var serviceAccountCredential = CredentialFactory.FromFile<ServiceAccountCredential>(jsonPath);
        var googleCredential = serviceAccountCredential.ToGoogleCredential();

        FirebaseApp.Create(new AppOptions
        {
            Credential = googleCredential
        });

        return services;
    }

    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<Repositories.Interfaces.IFacilityRepository, Repositories.FacilityRepository>();
        services.AddScoped<Repositories.Interfaces.IStatsRepository, Repositories.StatsRepository>();
        services.AddScoped<Repositories.Interfaces.IVaccineRepository, Repositories.VaccineRepository>();
        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<Services.Interfaces.IFacilityService, Services.FacilityService>();
        services.AddScoped<Services.Interfaces.IStatsService, Services.StatsService>();
        services.AddScoped<Services.Interfaces.IVaccineService, Services.VaccineService>();
        return services;
    }
}
