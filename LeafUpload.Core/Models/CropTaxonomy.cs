using System.Collections.Generic;

namespace LeafUpload.Core.Models
{
    // The families group crops that get the same weather-risk advice (e.g. all tree
    // fruit bruises/breaks the same way under hail) so StaticRuleAdvisoryGenerator can
    // give crop-aware guidance without needing a separate hand-written entry per crop.
    public enum CropFamily
    {
        TreeFruit,
        Vine,
        Berry,
        RowCrop,
    }

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

        public static readonly IReadOnlyDictionary<string, string> Icons = new Dictionary<string, string>
        {
            ["Apple"] = "🍎",
            ["Blueberry"] = "🫐",
            ["Cherry"] = "🍒",
            ["Corn (Maize)"] = "🌽",
            ["Grape"] = "🍇",
            ["Orange"] = "🍊",
            ["Peach"] = "🍑",
            ["Pepper (Bell)"] = "🫑",
            ["Potato"] = "🥔",
            ["Raspberry"] = "🍇",
            ["Soybean"] = "🌱",
            ["Squash"] = "🎃",
            ["Strawberry"] = "🍓",
            ["Tomato"] = "🍅",
        };

        public static readonly IReadOnlyDictionary<string, CropFamily> Families = new Dictionary<string, CropFamily>
        {
            ["Apple"] = CropFamily.TreeFruit,
            ["Cherry"] = CropFamily.TreeFruit,
            ["Orange"] = CropFamily.TreeFruit,
            ["Peach"] = CropFamily.TreeFruit,
            ["Grape"] = CropFamily.Vine,
            ["Blueberry"] = CropFamily.Berry,
            ["Raspberry"] = CropFamily.Berry,
            ["Strawberry"] = CropFamily.Berry,
            ["Corn (Maize)"] = CropFamily.RowCrop,
            ["Pepper (Bell)"] = CropFamily.RowCrop,
            ["Potato"] = CropFamily.RowCrop,
            ["Soybean"] = CropFamily.RowCrop,
            ["Squash"] = CropFamily.RowCrop,
            ["Tomato"] = CropFamily.RowCrop,
        };

        public static string IconFor(string cropType) => Icons.TryGetValue(cropType, out var icon) ? icon : "🌱";

        public static CropFamily FamilyFor(string cropType) =>
            Families.TryGetValue(cropType, out var family) ? family : CropFamily.RowCrop;

        // culture -> crop -> localized name (Hindi/Urdu). English uses the
        // canonical name in Crops above, so it isn't duplicated here.
        private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LocalizedNames =
            new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["hi"] = new Dictionary<string, string>
                {
                    ["Apple"] = "सेब",
                    ["Blueberry"] = "ब्लूबेरी",
                    ["Cherry"] = "चेरी",
                    ["Corn (Maize)"] = "मक्का",
                    ["Grape"] = "अंगूर",
                    ["Orange"] = "संतरा",
                    ["Peach"] = "आड़ू",
                    ["Pepper (Bell)"] = "शिमला मिर्च",
                    ["Potato"] = "आलू",
                    ["Raspberry"] = "रसभरी",
                    ["Soybean"] = "सोयाबीन",
                    ["Squash"] = "कद्दू",
                    ["Strawberry"] = "स्ट्रॉबेरी",
                    ["Tomato"] = "टमाटर",
                },
                ["ur"] = new Dictionary<string, string>
                {
                    ["Apple"] = "سیب",
                    ["Blueberry"] = "بلوبیری",
                    ["Cherry"] = "چیری",
                    ["Corn (Maize)"] = "مکئی",
                    ["Grape"] = "انگور",
                    ["Orange"] = "مالٹا",
                    ["Peach"] = "آڑو",
                    ["Pepper (Bell)"] = "شملہ مرچ",
                    ["Potato"] = "آلو",
                    ["Raspberry"] = "رسبری",
                    ["Soybean"] = "سویابین",
                    ["Squash"] = "کدو",
                    ["Strawberry"] = "اسٹرابیری",
                    ["Tomato"] = "ٹماٹر",
                },
            };

        public static string NameFor(string cropType, string cultureCode)
        {
            if (LocalizedNames.TryGetValue(cultureCode, out var names) && names.TryGetValue(cropType, out var localized))
                return localized;
            return cropType;
        }
    }
}
