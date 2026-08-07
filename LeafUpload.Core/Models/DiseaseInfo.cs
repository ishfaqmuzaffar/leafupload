using System.Collections.Generic;

namespace LeafUpload.Core.Models
{
    public class DiseaseInfo
    {
        // Other signs the farmer can check for themselves to confirm the
        // model's call before acting on it - the model gives one photo's
        // best guess, not a lab diagnosis.
        public IReadOnlyList<string> Symptoms { get; set; } = System.Array.Empty<string>();
        public string Treatment { get; set; } = string.Empty;
    }
}
