using System.Collections.Generic;

namespace LeafUpload.Core.Models
{
    // The 14 crops the leaf-disease model already covers, in farmer-readable form.
    // Deliberately not derived from the ML label taxonomy at runtime - see the
    // crop-advisories implementation plan for why.
    public static class CropTaxonomy
    {
        public static readonly IReadOnlyList<string> Crops = new List<string>
        {
            "Apple",
            "Blueberry",
            "Cherry",
            "Corn (Maize)",
            "Grape",
            "Orange",
            "Peach",
            "Pepper (Bell)",
            "Potato",
            "Raspberry",
            "Soybean",
            "Squash",
            "Strawberry",
            "Tomato",
        };
    }
}
