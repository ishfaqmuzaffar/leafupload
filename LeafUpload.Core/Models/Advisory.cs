using System;

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

        public string? ModelUsed { get; set; }
    }
}
