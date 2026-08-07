using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace LeafUpload.Core.Models
{
    public class Advisory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid FarmId { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public string AdvisoryText { get; set; } = string.Empty;

        // Raw snapshot of the weather data this advisory was generated from - audit trail.
        public string ForecastSummaryJson { get; set; } = string.Empty;

        // Serialized List<AdvisoryAlert> - kept as JSON text (not a navigation property)
        // since alerts are a value produced by whichever IAdvisoryGenerator ran, not an
        // independently queryable entity.
        public string? AlertsJson { get; set; }

        public string? ModelUsed { get; set; }

        [NotMapped]
        public List<AdvisoryAlert> Alerts =>
            string.IsNullOrWhiteSpace(AlertsJson)
                ? new List<AdvisoryAlert>()
                : JsonSerializer.Deserialize<List<AdvisoryAlert>>(AlertsJson) ?? new List<AdvisoryAlert>();

        [NotMapped]
        public WeatherForecast? Forecast =>
            string.IsNullOrWhiteSpace(ForecastSummaryJson)
                ? null
                : JsonSerializer.Deserialize<WeatherForecast>(ForecastSummaryJson);
    }
}
