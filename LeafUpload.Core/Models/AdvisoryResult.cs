using System.Collections.Generic;

namespace LeafUpload.Core.Models
{
    public class AdvisoryResult
    {
        // Plain-language rollup - what gets persisted as Advisory.AdvisoryText and
        // returned to non-web clients (e.g. the mobile app) that don't render cards.
        public string Summary { get; set; } = string.Empty;

        public List<AdvisoryAlert> Alerts { get; set; } = new();
    }
}
