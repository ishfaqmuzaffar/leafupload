using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using System;
using System.Globalization;
using System.Linq;
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
                       "&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,windspeed_10m_max,precipitation_probability_max,relative_humidity_2m_max,weathercode" +
                       "&timezone=auto&forecast_days=7";

            // api.open-meteo.com intermittently drops the initial connection attempt
            // (SocketException/timeout) even though the service is up - a couple of
            // quick retries clears it almost every time, sparing farmers a blank
            // "advisory pending" from what's really just a transient network hiccup.
            ForecastResponse? response = null;
            for (var attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    response = await client.GetFromJsonAsync<ForecastResponse>(url);
                    break;
                }
                catch (HttpRequestException) when (attempt < 3)
                {
                    await Task.Delay(500 * attempt);
                }
            }

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
                HumidityMaxPct = daily.HumidityMax,
                WeatherCode = daily.WeatherCode,
            };
        }

        public async Task<string?> ReverseGeocodeAsync(double latitude, double longitude)
        {
            var client = _httpClientFactory.CreateClient("NominatimReverseGeocoding");
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);
            var url = $"reverse?format=jsonv2&lat={lat}&lon={lon}&zoom=14";

            try
            {
                var result = await client.GetFromJsonAsync<NominatimReverseResponse>(url);
                if (result?.Address == null)
                    return result?.DisplayName;

                var locality = result.Address.Town ?? result.Address.City ?? result.Address.Village ?? result.Address.County;
                var parts = new[] { locality, result.Address.State, result.Address.Country }
                    .Where(p => !string.IsNullOrWhiteSpace(p));
                var joined = string.Join(", ", parts);
                return string.IsNullOrWhiteSpace(joined) ? result.DisplayName : joined;
            }
            catch (HttpRequestException)
            {
                return null;
            }
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

            [JsonPropertyName("relative_humidity_2m_max")]
            public double[]? HumidityMax { get; set; }

            [JsonPropertyName("weathercode")]
            public int[]? WeatherCode { get; set; }
        }

        private class NominatimReverseResponse
        {
            [JsonPropertyName("display_name")]
            public string? DisplayName { get; set; }

            [JsonPropertyName("address")]
            public NominatimAddress? Address { get; set; }
        }

        private class NominatimAddress
        {
            [JsonPropertyName("village")]
            public string? Village { get; set; }

            [JsonPropertyName("town")]
            public string? Town { get; set; }

            [JsonPropertyName("city")]
            public string? City { get; set; }

            [JsonPropertyName("county")]
            public string? County { get; set; }

            [JsonPropertyName("state")]
            public string? State { get; set; }

            [JsonPropertyName("country")]
            public string? Country { get; set; }
        }
    }
}
