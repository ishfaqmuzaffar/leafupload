using System;

namespace LeafUpload.Core.Models
{
    // Shaped around Open-Meteo's "daily" forecast fields.
    public class WeatherForecast
    {
        public DateOnly[] Dates { get; set; } = Array.Empty<DateOnly>();
        public double[] TempMaxC { get; set; } = Array.Empty<double>();
        public double[] TempMinC { get; set; } = Array.Empty<double>();
        public double[] PrecipitationMm { get; set; } = Array.Empty<double>();
        public double[] WindSpeedMaxKmh { get; set; } = Array.Empty<double>();
        public double[]? PrecipitationProbabilityPct { get; set; }
    }
}
