using Serilog;
using Serilog.Exceptions;

namespace mSzczepienia.Api.Configurations;

public static class SerilogConfiguration
{
    public static void ConfigureLogger(this IHostBuilder hostBuilder, bool isDevelopment)
    {
        hostBuilder.ConfigureLogging((context, logging) => { logging.ClearProviders(); });

        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information() //Change to Debug for more info
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console();

        if (isDevelopment)
            loggerConfiguration.WriteTo.File(
                "Logs/mSzczepienia_.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                fileSizeLimitBytes: 100_000,
                rollOnFileSizeLimit: true);

        //Some solution for production env
        Log.Logger = loggerConfiguration.CreateLogger();

        hostBuilder.UseSerilog();
    }
}