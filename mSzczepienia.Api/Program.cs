using Microsoft.EntityFrameworkCore;
using mSzczepienia.Api;
using mSzczepienia.Api.Configurations;
using mSzczepienia.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<BlobStorageSettings>(
    builder.Configuration.GetSection(BlobStorageSettings.SectionName));
builder.Services.Configure<AzureOcrSettings>(
    builder.Configuration.GetSection(AzureOcrSettings.SectionName));

// Add services to the container.
// Skip DbContext registration for testing - will be overridden in test factory
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
}


builder.Services.AddScoped<AppDbContextSeed>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterServices(builder.Configuration);
builder.Host.ConfigureLogger(builder.Environment.IsDevelopment());

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Migration and seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        // Only migrate if not using in-memory database (for testing)
        var environment = services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsEnvironment("Testing"))
        {
            dbContext.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database Migration Error");
    }

    try
    {
        // Skip seeding in testing environment
        var environment = services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsEnvironment("Testing"))
        {
            var seeder = services.GetRequiredService<AppDbContextSeed>();
            seeder.Seed();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database Seeding Error");
    }

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
