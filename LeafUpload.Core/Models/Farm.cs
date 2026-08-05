using System;

namespace LeafUpload.Core.Models
{
    public class Farm
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid FarmerId { get; set; }

        // Raw place name as the farmer typed it (e.g. "Anantnag").
        public string PlaceName { get; set; } = string.Empty;

        // What the geocoder actually matched - shown back to the farmer for confidence.
        public string? ResolvedLocationName { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string CropType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
