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
        public double[]? HumidityMaxPct { get; set; }

        // WMO weather interpretation codes (https://open-meteo.com/en/docs) - lets the
        // advisory generator distinguish e.g. a hailstorm (96/99) from ordinary rain.
        public int[]? WeatherCode { get; set; }
    }
}
