using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Weather
{
    public class OpenMeteoWeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OpenMeteoWeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<GeoLocation?> GeocodeAsync(string placeName)
        {
            var client = _httpClientFactory.CreateClient("OpenMeteoGeocoding");
            var url = $"search?name={Uri.EscapeDataString(placeName)}&count=1";

            var response = await client.GetFromJsonAsync<GeocodingResponse>(url);
            var match = response?.Results?.Length > 0 ? response.Results[0] : null;
            if (match == null)
                return null;

            var displayName = match.Admin1 != null
                ? $"{match.Name}, {match.Admin1}, {match.Country}"
                : $"{match.Name}, {match.Country}";

            return new GeoLocation
            {
                Latitude = match.Latitude,
                Longitude = match.Longitude,
                ResolvedName = displayName,
            };
        }

        public async Task<WeatherForecast> GetForecastAsync(double latitude, double longitude)
        {
            var client = _httpClientFactory.CreateClient("OpenMeteoForecast");
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);
            var url = "forecast?latitude=" + lat + "&longitude=" + lon +
                       "&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,windspeed_10m_max,precipitation_probability_max" +
                       "&timezone=auto&forecast_days=7";

            var response = await client.GetFromJsonAsync<ForecastResponse>(url);
            var daily = response?.Daily;
            if (daily == null)
                return new WeatherForecast();

            return new WeatherForecast
            {
                Dates = Array.ConvertAll(daily.Time ?? Array.Empty<string>(), DateOnly.Parse),
                TempMaxC = daily.TemperatureMax ?? Array.Empty<double>(),
                TempMinC = daily.TemperatureMin ?? Array.Empty<double>(),
                PrecipitationMm = daily.PrecipitationSum ?? Array.Empty<double>(),
                WindSpeedMaxKmh = daily.WindSpeedMax ?? Array.Empty<double>(),
                PrecipitationProbabilityPct = daily.PrecipitationProbabilityMax,
            };
        }

        private class GeocodingResponse
        {
            [JsonPropertyName("results")]
            public GeocodingResult[]? Results { get; set; }
        }

        private class GeocodingResult
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("latitude")]
            public double Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double Longitude { get; set; }

            [JsonPropertyName("admin1")]
            public string? Admin1 { get; set; }

            [JsonPropertyName("country")]
            public string? Country { get; set; }
        }

        private class ForecastResponse
        {
            [JsonPropertyName("daily")]
            public DailyForecast? Daily { get; set; }
        }

        private class DailyForecast
        {
            [JsonPropertyName("time")]
            public string[]? Time { get; set; }

            [JsonPropertyName("temperature_2m_max")]
            public double[]? TemperatureMax { get; set; }

            [JsonPropertyName("temperature_2m_min")]
            public double[]? TemperatureMin { get; set; }

            [JsonPropertyName("precipitation_sum")]
            public double[]? PrecipitationSum { get; set; }

            [JsonPropertyName("windspeed_10m_max")]
            public double[]? WindSpeedMax { get; set; }

            [JsonPropertyName("precipitation_probability_max")]
            public double[]? PrecipitationProbabilityMax { get; set; }
        }
    }
}
