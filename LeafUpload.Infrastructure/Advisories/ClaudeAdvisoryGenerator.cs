using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Advisories
{
    public class ClaudeAdvisoryGenerator : IAdvisoryGenerator
    {
        private const string FallbackMessage =
            "Advisory generation is temporarily unavailable. Please check back later, or contact your local agriculture office for guidance.";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _model;
        private readonly ILogger<ClaudeAdvisoryGenerator> _logger;

        public ClaudeAdvisoryGenerator(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ClaudeAdvisoryGenerator> logger)
        {
            _httpClientFactory = httpClientFactory;
            _model = configuration["Anthropic:Model"] ?? "claude-sonnet-5";
            _logger = logger;
        }

        public async Task<AdvisoryResult> GenerateAdvisoryAsync(string cropType, string placeName, WeatherForecast forecast)
        {
            var text = await GenerateTextAsync(cropType, placeName, forecast);
            return new AdvisoryResult
            {
                Summary = text,
                Alerts = new List<AdvisoryAlert>
                {
                    new()
                    {
                        Icon = "✨",
                        Title = "AI-Generated Advisory",
                        Severity = AdvisorySeverity.Info,
                        Message = text,
                    },
                },
            };
        }

        private async Task<string> GenerateTextAsync(string cropType, string placeName, WeatherForecast forecast)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AnthropicClient");
                var prompt = BuildPrompt(cropType, placeName, forecast);

                var requestBody = new
                {
                    model = _model,
                    max_tokens = 400,
                    messages = new[] { new { role = "user", content = prompt } },
                };

                var response = await client.PostAsJsonAsync("messages", requestBody);
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Anthropic API returned {StatusCode}: {Body}", response.StatusCode, errorBody);
                    return FallbackMessage;
                }

                var result = await response.Content.ReadFromJsonAsync<MessagesResponse>();
                var text = result?.Content?.Length > 0 ? result.Content[0].Text : null;
                return string.IsNullOrWhiteSpace(text) ? FallbackMessage : text.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to generate advisory via Anthropic API.");
                return FallbackMessage;
            }
        }

        private static string BuildPrompt(string cropType, string placeName, WeatherForecast forecast)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"You are an agricultural advisor helping a farmer in {placeName} growing {cropType}.");
            sb.AppendLine();
            sb.AppendLine("Here is the 7-day weather forecast:");
            for (var i = 0; i < forecast.Dates.Length; i++)
            {
                var precipProb = forecast.PrecipitationProbabilityPct != null && i < forecast.PrecipitationProbabilityPct.Length
                    ? $", {forecast.PrecipitationProbabilityPct[i]:0}% chance of rain"
                    : string.Empty;
                var humidity = forecast.HumidityMaxPct != null && i < forecast.HumidityMaxPct.Length
                    ? $", {forecast.HumidityMaxPct[i]:0}% humidity"
                    : string.Empty;
                var condition = forecast.WeatherCode != null && i < forecast.WeatherCode.Length
                    ? $", conditions: {DescribeWeatherCode(forecast.WeatherCode[i])}"
                    : string.Empty;
                sb.AppendLine($"- {forecast.Dates[i]:yyyy-MM-dd}: {forecast.TempMinC[i]:0.#}-{forecast.TempMaxC[i]:0.#}°C, " +
                               $"{forecast.PrecipitationMm[i]:0.#}mm rain{precipProb}, wind up to {forecast.WindSpeedMaxKmh[i]:0.#}km/h" +
                               $"{humidity}{condition}");
            }
            sb.AppendLine();
            sb.AppendLine("Based on this forecast and the crop being grown, write a short, practical crop advisory " +
                           "(3-5 sentences) covering irrigation timing, disease/pest risk from the weather conditions " +
                           "(e.g. high humidity or rain increasing fungal disease risk), and any urgent action needed " +
                           "in the next few days. Explicitly call out and prioritize any severe or crop-damaging events " +
                           "in the forecast - hail or thunderstorms (protective netting, early harvest, sheltering livestock), " +
                           "heat waves or sustained high temperatures (irrigation timing, scorch risk), and frost or " +
                           "near-freezing lows (frost protection) - naming the specific day(s) they're expected. " +
                           "Write in plain, farmer-friendly language with no markdown formatting.");
            return sb.ToString();
        }

        // WMO weather codes used by Open-Meteo (https://open-meteo.com/en/docs) - only the
        // ranges relevant to farmer-facing advisories are distinguished here.
        private static string DescribeWeatherCode(int code) => code switch
        {
            0 => "clear sky",
            1 or 2 or 3 => "partly cloudy",
            45 or 48 => "fog",
            51 or 53 or 55 => "light drizzle",
            56 or 57 => "freezing drizzle",
            61 or 63 or 65 => "rain",
            66 or 67 => "freezing rain",
            71 or 73 or 75 or 77 => "snow",
            80 or 81 or 82 => "rain showers",
            85 or 86 => "snow showers",
            95 => "thunderstorm",
            96 or 99 => "thunderstorm with HAIL",
            _ => "unsettled weather",
        };

        private class MessagesResponse
        {
            [JsonPropertyName("content")]
            public ContentBlock[]? Content { get; set; }
        }

        private class ContentBlock
        {
            [JsonPropertyName("type")]
            public string? Type { get; set; }

            [JsonPropertyName("text")]
            public string? Text { get; set; }
        }
    }
}
