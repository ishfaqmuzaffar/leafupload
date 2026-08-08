using LeafUpload.Core.Abstractions;

namespace LeafUpload.Web.Services
{
    // Proactively checks every farm's forecast on a timer, instead of only when a
    // farmer happens to open the Crop Advisories tab, so a severe alert (e.g. a
    // cloudburst forecast for Pahalgam) reaches affected farmers as a push
    // notification even if they never open the app that day.
    //
    // Reuses FarmAdvisoryService.GetOrGenerateAdvisoryAsync as-is - all the
    // generation, caching, and "notify on Warning/Critical" logic already lives
    // there, this just drives it across every farm on a schedule instead of one
    // farm on request.
    public class WeatherAlertSweepService : BackgroundService
    {
        private static readonly TimeSpan SweepInterval = TimeSpan.FromHours(4);

        // Shorter than FarmAdvisoryService's default 24h on-demand cache, since a
        // sweep's whole purpose is catching a newly-issued alert well within a day.
        private static readonly TimeSpan SweepFreshnessWindow = TimeSpan.FromHours(4);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WeatherAlertSweepService> _logger;

        public WeatherAlertSweepService(IServiceScopeFactory scopeFactory, ILogger<WeatherAlertSweepService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(SweepInterval);
            do
            {
                await SweepAsync(stoppingToken);
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task SweepAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var farmerRepository = scope.ServiceProvider.GetRequiredService<IFarmerRepository>();
            var advisoryService = scope.ServiceProvider.GetRequiredService<FarmAdvisoryService>();

            var farms = await farmerRepository.GetAllFarmsAsync();
            _logger.LogInformation("Weather alert sweep starting for {FarmCount} farms.", farms.Count);

            foreach (var farm in farms)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (farm.Latitude == null || farm.Longitude == null)
                    continue;

                try
                {
                    await advisoryService.GetOrGenerateAdvisoryAsync(farm, SweepFreshnessWindow);
                }
                catch (Exception ex)
                {
                    // One farm's failure (bad data, transient error not already
                    // swallowed inside GetOrGenerateAdvisoryAsync) shouldn't stop the
                    // rest of the sweep from checking every other farm.
                    _logger.LogWarning(ex, "Weather alert sweep failed for farm {FarmId}.", farm.Id);
                }
            }
        }
    }
}
