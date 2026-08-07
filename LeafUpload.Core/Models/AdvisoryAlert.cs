using System.Collections.Generic;

namespace LeafUpload.Core.Models
{
    // info < caution < warning < critical
    public enum AdvisorySeverity
    {
        Info,
        Caution,
        Warning,
        Critical,
    }

    public class AdvisoryAlert
    {
        public string Icon { get; set; } = "ℹ️";
        public string Title { get; set; } = string.Empty;
        public AdvisorySeverity Severity { get; set; } = AdvisorySeverity.Info;
        public string Message { get; set; } = string.Empty;
        public List<string> Actions { get; set; } = new();

        // e.g. "Aug 8 - Aug 11" - the specific forecast days this alert applies to, if any.
        public string? Timing { get; set; }
    }
}
