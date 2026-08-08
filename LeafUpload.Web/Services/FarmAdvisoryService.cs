using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using System.Text.Json;

namespace LeafUpload.Web.Services
{
    // Shared by the MVC AdvisoryController (web) and the api/mobile advisory endpoint
    // so the 24h-cache-then-generate-then-fallback-on-error logic lives in one place.
    public class FarmAdvisoryService
    {
        private static readonly TimeSpan DefaultFreshnessWindow = TimeSpan.FromHours(24);

        private readonly IAdvisoryRepository _advisoryRepository;
        private readonly IWeatherService _weatherService;
        private readonly IAdvisoryGenerator _advisoryGenerator;
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ILogger<FarmAdvisoryService> _logger;

        public FarmAdvisoryService(
            IAdvisoryRepository advisoryRepository,
            IWeatherService weatherService,
            IAdvisoryGenerator advisoryGenerator,
            IDeviceTokenRepository deviceTokenRepository,
            IPushNotificationService pushNotificationService,
            ILogger<FarmAdvisoryService> logger)
        {
            _advisoryRepository = advisoryRepository;
            _weatherService = weatherService;
            _advisoryGenerator = advisoryGenerator;
            _deviceTokenRepository = deviceTokenRepository;
            _pushNotificationService = pushNotificationService;
            _logger = logger;
        }

        // freshnessWindow lets callers other than the interactive "farmer opened the
        // app" path (e.g. the background weather sweep, which wants to notice a
        // newly-issued severe alert well before 24h are up) ask for a shorter cache
        // lifetime without affecting the default on-demand behavior.
        public async Task<Advisory?> GetOrGenerateAdvisoryAsync(Farm farm, TimeSpan? freshnessWindow = null)
        {
            var window = freshnessWindow ?? DefaultFreshnessWindow;
            var existing = await _advisoryRepository.GetLatestAdvisoryForFarmAsync(farm.Id);
            if (existing != null && DateTime.UtcNow - existing.GeneratedAt < window)
                return existing;

            // No coordinates to fetch weather with - shouldn't happen for farms
            // registered after geocoding was wired up, but guard anyway.
            if (farm.Latitude == null || farm.Longitude == null)
                return existing;

            // The Anthropic call already degrades to a fallback message internally
            // and never throws, but the weather fetch can still fail on a network
            // hiccup or Open-Meteo outage - a farmer should see a stale-but-present
            // advisory (or a clear "unavailable" state) rather than a crashed page.
            try
            {
                var forecast = await _weatherService.GetForecastAsync(farm.Latitude.Value, farm.Longitude.Value);
                var locationLabel = farm.ResolvedLocationName ?? farm.PlaceName;
                var result = await _advisoryGenerator.GenerateAdvisoryAsync(farm.CropType, locationLabel, forecast);

                var advisory = new Advisory
                {
                    FarmId = farm.Id,
                    AdvisoryText = result.Summary,
                    AlertsJson = JsonSerializer.Serialize(result.Alerts),
                    ForecastSummaryJson = JsonSerializer.Serialize(forecast),
                    ModelUsed = _advisoryGenerator.GetType().Name,
                };
                await _advisoryRepository.SaveAdvisoryAsync(advisory);
                await NotifyIfSevereAsync(farm, result.Alerts);
                return advisory;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to refresh advisory for farm {FarmId}; falling back to last known advisory.", farm.Id);
                return existing;
            }
        }

        // Only fires for a freshly-generated advisory (not a cached one returned
        // above), so a farmer isn't re-notified about the same alert all day.
        private async Task NotifyIfSevereAsync(Farm farm, List<AdvisoryAlert> alerts)
        {
            var worstAlert = alerts
                .Where(a => a.Severity is AdvisorySeverity.Warning or AdvisorySeverity.Critical)
                .OrderByDescending(a => (int)a.Severity)
                .FirstOrDefault();

            if (worstAlert == null)
                return;

            var tokens = await _deviceTokenRepository.GetTokensForFarmerAsync(farm.FarmerId);
            if (tokens.Count == 0)
                return;

            var title = $"{worstAlert.Icon} {worstAlert.Title} - {farm.ResolvedLocationName ?? farm.PlaceName}";
            await _pushNotificationService.SendToTokensAsync(tokens.Select(t => t.Token), title, worstAlert.Message);
        }
    }
}
