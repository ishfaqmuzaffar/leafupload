using System.Collections.Generic;

namespace LeafUpload.Web.ViewModels
{
    public class LandingViewModel
    {
        public int FarmerCount { get; set; }
        public int FarmCount { get; set; }
        public int CropVarietyCount { get; set; }
        public int ActiveAlertCount { get; set; }

        // Crop name -> number of registered farms growing it.
        public Dictionary<string, int> CropCounts { get; set; } = new();

        // Alert title (e.g. "Hail Risk") -> number of farms currently facing it,
        // based on each farm's most recent advisory.
        public Dictionary<string, int> AlertCounts { get; set; } = new();
    }
}
