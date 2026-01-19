using mSzczepienia.Api.Services.Interfaces;

namespace mSzczepienia.Api.Workers
{
    public class ReminderWorker: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReminderWorker> _logger;

        private readonly TimeSpan _scheduledTime = new TimeSpan(9, 0, 0);

        public ReminderWorker(IServiceScopeFactory scopeFactory, ILogger<ReminderWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Service is running");

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = CalculateDelayToNextRun();
                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                await ProcessNotificationsAsync(stoppingToken);
            }
        }

        private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var vaccinationService = scope.ServiceProvider.GetRequiredService<IVaccinationRecordService>();

                _logger.LogInformation("Processing notifications...");

                await vaccinationService.RemindAboutNextVaccinationsAsync(stoppingToken);
            }
        }

        private TimeSpan CalculateDelayToNextRun()
        {
            var now = DateTime.Now;
            var nextRun = now.Date + _scheduledTime; // Today at 9 AM

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1); // If it's past 9 AM, schedule for next day
            }

            return nextRun - now;
        }
    }
}