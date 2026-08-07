using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Advisories
{
    // Deterministic, no-API-call advisory engine. Reads the same WeatherForecast the
    // Claude-backed generator uses and turns it into severity-ranked alerts via fixed
    // thresholds + crop-family-specific tips, rather than an LLM call. Swapped in as the
    // default IAdvisoryGenerator while the Anthropic account has no credit - see Program.cs.
    public class StaticRuleAdvisoryGenerator : IAdvisoryGenerator
    {
        private const int HailCodeSlight = 96;
        private const int HailCodeHeavy = 99;
        private const int ThunderstormCode = 95;

        public Task<AdvisoryResult> GenerateAdvisoryAsync(string cropType, string placeName, WeatherForecast forecast)
        {
            var family = CropTaxonomy.FamilyFor(cropType);
            var alerts = new List<AdvisoryAlert>();

            AddHailOrThunderstormAlert(forecast, family, alerts);
            AddFrostAlert(forecast, family, alerts);
            AddWindstormAlert(forecast, family, alerts);
            AddHeavyRainAlert(forecast, family, alerts);
            AddHeatWaveAlert(forecast, family, alerts);
            AddHighHumidityAlert(forecast, family, alerts);

            if (alerts.Count == 0)
            {
                alerts.Add(new AdvisoryAlert
                {
                    Icon = "☀️",
                    Title = Title("favorable"),
                    Severity = AdvisorySeverity.Info,
                    Message = Culture switch
                    {
                        "hi" => $"अगले 7 दिनों में आपकी {cropType} फ़सल के लिए कोई गंभीर मौसम खतरा नहीं है - खेत के सामान्य काम, निरीक्षण और देखभाल के लिए अनुकूल समय है।",
                        "ur" => $"اگلے 7 دنوں میں آپ کی {cropType} فصل کے لیے کوئی شدید موسمی خطرہ نہیں ہے - کھیت کے معمول کے کام، معائنے اور دیکھ بھال کے لیے موزوں وقت ہے۔",
                        _ => $"No severe weather is expected for your {cropType.ToLowerInvariant()} over the next 7 days - good conditions for routine field work, scouting, and maintenance.",
                    },
                });
            }

            alerts.Add(BuildIrrigationAlert(forecast));

            var today = forecast.Dates.Length > 0 ? forecast.Dates[0] : DateOnly.FromDateTime(DateTime.UtcNow);
            alerts.Add(BuildSeasonStatusAlert(cropType, family, today));

            alerts = alerts.OrderByDescending(a => (int)a.Severity).ToList();

            var summary = BuildSummary(cropType, placeName, alerts);
            return Task.FromResult(new AdvisoryResult { Summary = summary, Alerts = alerts });
        }

        private static void AddHailOrThunderstormAlert(WeatherForecast forecast, CropFamily family, List<AdvisoryAlert> alerts)
        {
            if (forecast.WeatherCode == null)
                return;

            var hailDays = DaysWhere(forecast, i => forecast.WeatherCode[i] is HailCodeSlight or HailCodeHeavy);
            if (hailDays.Count > 0)
            {
                alerts.Add(new AdvisoryAlert
                {
                    Icon = "🧊",
                    Title = Title("hail"),
                    Severity = AdvisorySeverity.Critical,
                    Message = $"Thunderstorms with hail are forecast. {FamilyTip(family, HailTips)}",
                    Actions = FamilyActions(family, HailActions),
                    Timing = FormatTiming(forecast, hailDays),
                });
                return;
            }

            var stormDays = DaysWhere(forecast, i => forecast.WeatherCode[i] == ThunderstormCode);
            if (stormDays.Count > 0)
            {
                alerts.Add(new AdvisoryAlert
                {
                    Icon = "⛈️",
                    Title = Title("thunderstorm"),
                    Severity = AdvisorySeverity.Warning,
                    Message = "Thunderstorms are forecast - secure loose equipment and avoid spraying or fertilizing right before storms hit, since rain will wash treatments off.",
                    Actions = new List<string> { "Delay spraying until after the storm window", "Secure trellises, tunnels, and loose equipment" },
                    Timing = FormatTiming(forecast, stormDays),
                });
            }
        }

        private static void AddFrostAlert(WeatherForecast forecast, CropFamily family, List<AdvisoryAlert> alerts)
        {
            var freezingDays = DaysWhere(forecast, i => forecast.TempMinC[i] <= 0);
            var frostDays = DaysWhere(forecast, i => forecast.TempMinC[i] <= 2);
            if (frostDays.Count == 0)
                return;

            alerts.Add(new AdvisoryAlert
            {
                Icon = "🥶",
                Title = Title("frost"),
                Severity = freezingDays.Count > 0 ? AdvisorySeverity.Critical : AdvisorySeverity.Warning,
                Message = $"Overnight temperatures are expected to drop near or below freezing. {FamilyTip(family, FrostTips)}",
                Actions = FamilyActions(family, FrostActions),
                Timing = FormatTiming(forecast, frostDays),
            });
        }

        private static void AddWindstormAlert(WeatherForecast forecast, CropFamily family, List<AdvisoryAlert> alerts)
        {
            var severeDays = DaysWhere(forecast, i => forecast.WindSpeedMaxKmh[i] >= 60);
            var windyDays = DaysWhere(forecast, i => forecast.WindSpeedMaxKmh[i] >= 40);
            if (windyDays.Count == 0)
                return;

            alerts.Add(new AdvisoryAlert
            {
                Icon = "💨",
                Title = Title("wind"),
                Severity = severeDays.Count > 0 ? AdvisorySeverity.Critical : AdvisorySeverity.Warning,
                Message = $"Damaging winds are forecast. {FamilyTip(family, WindTips)}",
                Actions = FamilyActions(family, WindActions),
                Timing = FormatTiming(forecast, windyDays),
            });
        }

        private static void AddHeavyRainAlert(WeatherForecast forecast, CropFamily family, List<AdvisoryAlert> alerts)
        {
            var heavyDays = DaysWhere(forecast, i => forecast.PrecipitationMm[i] >= 30);
            var weeklyTotal = forecast.PrecipitationMm.Sum();
            if (heavyDays.Count == 0 && weeklyTotal < 80)
                return;

            var severity = heavyDays.Any(i => forecast.PrecipitationMm[i] >= 50) || weeklyTotal >= 120
                ? AdvisorySeverity.Critical
                : AdvisorySeverity.Warning;

            alerts.Add(new AdvisoryAlert
            {
                Icon = "🌧️",
                Title = Title("rain"),
                Severity = severity,
                Message = $"Heavy rain is forecast and waterlogging is a real risk. {FamilyTip(family, RainTips)}",
                Actions = FamilyActions(family, RainActions),
                Timing = heavyDays.Count > 0 ? FormatTiming(forecast, heavyDays) : null,
            });
        }

        private static void AddHeatWaveAlert(WeatherForecast forecast, CropFamily family, List<AdvisoryAlert> alerts)
        {
            var extremeDays = DaysWhere(forecast, i => forecast.TempMaxC[i] >= 40);
            var hotDays = DaysWhere(forecast, i => forecast.TempMaxC[i] >= 35);
            if (hotDays.Count == 0)
                return;

            var severity = extremeDays.Count > 0 || hotDays.Count >= 2 ? AdvisorySeverity.Warning : AdvisorySeverity.Caution;
            if (extremeDays.Count >= 2)
                severity = AdvisorySeverity.Critical;

            alerts.Add(new AdvisoryAlert
            {
                Icon = "🌡️",
                Title = Title("heat"),
                Severity = severity,
                Message = $"Sustained high temperatures are forecast, raising heat stress and water demand. {FamilyTip(family, HeatTips)}",
                Actions = FamilyActions(family, HeatActions),
                Timing = FormatTiming(forecast, hotDays),
            });
        }

        private static void AddHighHumidityAlert(WeatherForecast forecast, CropFamily family, List<AdvisoryAlert> alerts)
        {
            if (forecast.HumidityMaxPct == null)
                return;

            var humidDays = DaysWhere(forecast, i => forecast.HumidityMaxPct[i] >= 85);
            if (humidDays.Count < 3)
                return;

            alerts.Add(new AdvisoryAlert
            {
                Icon = "💧",
                Title = Title("humidity"),
                Severity = humidDays.Count >= 5 ? AdvisorySeverity.Warning : AdvisorySeverity.Caution,
                Message = $"Prolonged high humidity favors fungal and bacterial disease. {FamilyTip(family, HumidityTips)}",
                Actions = FamilyActions(family, HumidityActions),
                Timing = FormatTiming(forecast, humidDays),
            });
        }

        private static AdvisoryAlert BuildIrrigationAlert(WeatherForecast forecast)
        {
            var weeklyTotal = forecast.PrecipitationMm.Sum();
            var mm = weeklyTotal.ToString("0.#");
            string message = Culture switch
            {
                "hi" => weeklyTotal < 10
                    ? $"इस सप्ताह केवल लगभग {mm}mm वर्षा की संभावना है - नमी की कमी से बचने के लिए सिंचाई की योजना बनाएं।"
                    : weeklyTotal > 60
                        ? $"इस सप्ताह लगभग {mm}mm वर्षा की संभावना है - जलभराव और जड़ सड़न से बचने के लिए अतिरिक्त सिंचाई रोकें।"
                        : $"इस सप्ताह लगभग {mm}mm वर्षा की संभावना है - यदि बारिश के बीच मिट्टी सूख जाए तो हल्की पूरक सिंचाई पर्याप्त होगी।",
                "ur" => weeklyTotal < 10
                    ? $"اس ہفتے صرف تقریباً {mm}mm بارش متوقع ہے - نمی کی کمی سے بچنے کے لیے آبپاشی کی منصوبہ بندی کریں۔"
                    : weeklyTotal > 60
                        ? $"اس ہفتے تقریباً {mm}mm بارش متوقع ہے - پانی جمع ہونے اور جڑوں کی بیماری سے بچنے کے لیے اضافی آبپاشی روک دیں۔"
                        : $"اس ہفتے تقریباً {mm}mm بارش متوقع ہے - اگر بارش کے درمیان مٹی خشک ہو جائے تو ہلکی اضافی آبپاشی کافی ہوگی۔",
                _ => weeklyTotal < 10
                    ? $"Only about {mm}mm of rain is expected this week - plan to irrigate to avoid moisture stress."
                    : weeklyTotal > 60
                        ? $"About {mm}mm of rain is expected this week - hold off on extra irrigation to avoid waterlogging and root disease."
                        : $"About {mm}mm of rain is expected this week - light supplemental irrigation should be enough if the soil dries out between showers.",
            };

            return new AdvisoryAlert
            {
                Icon = "🚰",
                Title = Title("irrigation"),
                Severity = AdvisorySeverity.Info,
                Message = message,
            };
        }

        // Tells the farmer where their crop sits in its growing cycle (bloom, ripening,
        // harvest, dormant, etc.) independent of the 7-day weather forecast - e.g. so a
        // cherry farmer checking in July sees that harvest already ended and the trees
        // won't fruit again until next year, rather than only ever seeing weather risk.
        private static AdvisoryAlert BuildSeasonStatusAlert(string cropType, CropFamily family, DateOnly today)
        {
            var phase = CropCalendar.GetPhase(cropType, today);
            var isPerennial = family != CropFamily.RowCrop;
            var noun = family switch
            {
                CropFamily.TreeFruit => "trees",
                CropFamily.Vine => "vines",
                CropFamily.Berry => "plants",
                _ => "crop",
            };
            var cropLower = cropType.ToLowerInvariant();

            string icon;
            string message;
            switch (phase)
            {
                case SeasonPhase.OffSeason:
                    icon = "🍂";
                    message = isPerennial
                        ? $"It's the dormant season for {cropLower} {noun} - a good window for pruning, sanitation, and other maintenance before growth resumes in spring."
                        : $"This isn't the growing season for {cropLower} in most of Jammu & Kashmir - the next planting window typically opens around {CropCalendar.NextGrowingMonthName(cropType)}.";
                    break;
                case SeasonPhase.Growing:
                    icon = isPerennial ? "🌸" : "🌱";
                    message = isPerennial
                        ? $"{cropType} {noun} are coming into growth and bloom now - watch overnight temperatures closely, since a late frost during flowering can wipe out this year's fruit set."
                        : $"This is the planting and early-growth window for {cropLower} - get seed or transplants in if you haven't already, and keep soil moisture consistent while the crop establishes.";
                    break;
                case SeasonPhase.Ripening:
                    icon = CropTaxonomy.IconFor(cropType);
                    message = isPerennial
                        ? $"{cropType} fruit is developing on the {noun} now - keep up irrigation and pest scouting as it sizes up toward harvest."
                        : $"{cropType} is filling out and ripening now - keep up irrigation and pest scouting as it heads toward harvest.";
                    break;
                case SeasonPhase.Harvest:
                    icon = CropTaxonomy.IconFor(cropType);
                    message = $"This is harvest season for {cropLower} - plan labor, storage, and market logistics accordingly.";
                    break;
                default: // PostHarvest - perennial-only phase
                    icon = "🍂";
                    message = $"{cropType} harvest has ended for this year - the {noun} won't bear fruit again until next year's bloom. This is a good time for post-harvest pruning, fertilizing, and pest cleanup to set up next season.";
                    break;
            }

            return new AdvisoryAlert
            {
                Icon = icon,
                Title = SeasonTitle(phase),
                Severity = AdvisorySeverity.Info,
                Message = message,
            };
        }

        private static string SeasonTitle(SeasonPhase phase) => phase switch
        {
            SeasonPhase.OffSeason => Culture switch { "hi" => "मौसम स्थिति: ऑफ़-सीज़न", "ur" => "موسمی صورتحال: آف سیزن", _ => "Season Status: Off-Season" },
            SeasonPhase.Growing => Culture switch { "hi" => "मौसम स्थिति: वृद्धि काल", "ur" => "موسمی صورتحال: نشوونما کا دور", _ => "Season Status: Growing" },
            SeasonPhase.Ripening => Culture switch { "hi" => "मौसम स्थिति: पकने का समय", "ur" => "موسمی صورتحال: پکنے کا وقت", _ => "Season Status: Ripening" },
            SeasonPhase.Harvest => Culture switch { "hi" => "मौसम स्थिति: कटाई का समय", "ur" => "موسمی صورتحال: کٹائی کا وقت", _ => "Season Status: Harvest Time" },
            _ => Culture switch { "hi" => "मौसम स्थिति: कटाई पूर्ण", "ur" => "موسمی صورتحال: کٹائی مکمل", _ => "Season Status: Harvest Complete" },
        };

        private static string BuildSummary(string cropType, string placeName, List<AdvisoryAlert> alerts)
        {
            var leadAlerts = alerts.Where(a => a.Severity != AdvisorySeverity.Info).Take(3).ToList();
            if (leadAlerts.Count == 0)
                leadAlerts = alerts.Take(1).ToList();

            var lead = string.Join(" ", leadAlerts.Select(a => a.Message));
            return $"{cropType} in {placeName}: {lead}";
        }

        private static List<int> DaysWhere(WeatherForecast forecast, Func<int, bool> predicate)
        {
            var days = new List<int>();
            for (var i = 0; i < forecast.Dates.Length; i++)
            {
                if (predicate(i))
                    days.Add(i);
            }
            return days;
        }

        private static string FormatTiming(WeatherForecast forecast, List<int> dayIndexes)
        {
            var dates = dayIndexes.Select(i => forecast.Dates[i]).ToList();
            if (dates.Count == 0)
                return string.Empty;
            if (dates.Count == 1)
                return dates[0].ToString("MMM d");
            return $"{dates.First():MMM d} - {dates.Last():MMM d}";
        }

        // Alert titles and crop-agnostic messages are translated; the detailed
        // per-crop-family tips/actions below remain English-only for now (a large
        // follow-up task - see project notes).
        private static string Culture => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        private static string Title(string key) => key switch
        {
            "hail" => Culture switch { "hi" => "ओलावृष्टि का खतरा", "ur" => "اولوں کا خطرہ", _ => "Hail Risk" },
            "thunderstorm" => Culture switch { "hi" => "आंधी-तूफान की संभावना", "ur" => "طوفانی بارش کا امکان", _ => "Thunderstorms Expected" },
            "frost" => Culture switch { "hi" => "पाला पड़ने का खतरा", "ur" => "پالے کا خطرہ", _ => "Frost Risk" },
            "wind" => Culture switch { "hi" => "तेज़ हवाएँ", "ur" => "تیز ہوائیں", _ => "Strong Winds" },
            "rain" => Culture switch { "hi" => "भारी वर्षा", "ur" => "شدید بارش", _ => "Heavy Rainfall" },
            "heat" => Culture switch { "hi" => "लू / अत्यधिक गर्मी", "ur" => "شدید گرمی کی لہر", _ => "Heat Wave" },
            "humidity" => Culture switch { "hi" => "अधिक नमी - रोग का खतरा", "ur" => "زیادہ نمی - بیماری کا خطرہ", _ => "High Humidity - Disease Risk" },
            "favorable" => Culture switch { "hi" => "अनुकूल परिस्थितियाँ", "ur" => "سازگار حالات", _ => "Favorable Conditions" },
            "irrigation" => Culture switch { "hi" => "सिंचाई सलाह", "ur" => "آبپاشی کی ہدایت", _ => "Irrigation Guidance" },
            _ => key,
        };

        private static string FamilyTip(CropFamily family, IReadOnlyDictionary<CropFamily, string> tips) =>
            tips.TryGetValue(family, out var tip) ? tip : tips[CropFamily.RowCrop];

        private static List<string> FamilyActions(CropFamily family, IReadOnlyDictionary<CropFamily, List<string>> actions) =>
            new(actions.TryGetValue(family, out var list) ? list : actions[CropFamily.RowCrop]);

        private static readonly Dictionary<CropFamily, string> HailTips = new()
        {
            [CropFamily.TreeFruit] = "Bruised or split fruit won't store well, so prioritize harvesting exposed fruit before the storm if it's close to ripe.",
            [CropFamily.Vine] = "Hail shreds leaves and scars berries fast, hurting this season's yield and next year's canopy - deploy overhead netting now if you have it.",
            [CropFamily.Berry] = "Low canes and beds are easiest to protect - pull row covers over the plants before the storm arrives.",
            [CropFamily.RowCrop] = "Hail can shred leaves and snap stalks - after the storm, walk the field to check plant stand and be ready to assess replanting if damage is severe.",
        };
        private static readonly Dictionary<CropFamily, List<string>> HailActions = new()
        {
            [CropFamily.TreeFruit] = new() { "Deploy hail netting if available", "Harvest ripe or near-ripe fruit before the storm", "Inspect limbs for breakage afterward" },
            [CropFamily.Vine] = new() { "Deploy overhead netting if available", "Check trellis wires and posts afterward" },
            [CropFamily.Berry] = new() { "Cover beds with row cover or tunnels", "Check for flattened canes afterward" },
            [CropFamily.RowCrop] = new() { "Avoid field work during the storm", "Assess stand loss and replanting need afterward" },
        };

        private static readonly Dictionary<CropFamily, string> FrostTips = new()
        {
            [CropFamily.TreeFruit] = "Blossoms and young fruit are especially frost-sensitive - run wind machines or irrigation-based frost protection if trees are flowering or fruiting.",
            [CropFamily.Vine] = "New shoots can be killed outright by frost - consider overhead irrigation or wind machines during the coldest pre-dawn hours.",
            [CropFamily.Berry] = "Open blossoms are the most frost-sensitive stage - cover plants overnight if flowering has started.",
            [CropFamily.RowCrop] = "Cover young seedlings overnight and delay any planned transplanting until temperatures recover.",
        };
        private static readonly Dictionary<CropFamily, List<string>> FrostActions = new()
        {
            [CropFamily.TreeFruit] = new() { "Run frost protection (wind machines/irrigation) overnight if blooming", "Delay pruning until after the cold snap" },
            [CropFamily.Vine] = new() { "Run frost protection overnight if shoots have emerged" },
            [CropFamily.Berry] = new() { "Cover flowering plants overnight", "Delay uncovering until mid-morning" },
            [CropFamily.RowCrop] = new() { "Cover young seedlings overnight", "Delay transplanting until the cold snap passes" },
        };

        private static readonly Dictionary<CropFamily, string> WindTips = new()
        {
            [CropFamily.TreeFruit] = "Strong wind can strip fruit and snap weakly attached limbs - check stakes and props on young trees.",
            [CropFamily.Vine] = "Check trellis wires and end-post anchors before the wind picks up, and tie down loose canopy growth.",
            [CropFamily.Berry] = "Secure row covers and tunnel plastic - wind can tear or lift them off the plants.",
            [CropFamily.RowCrop] = "Tall or top-heavy crops can lodge in strong wind - avoid spraying (drift risk) until winds ease.",
        };
        private static readonly Dictionary<CropFamily, List<string>> WindActions = new()
        {
            [CropFamily.TreeFruit] = new() { "Check stakes/props on young trees", "Delay spraying until winds ease" },
            [CropFamily.Vine] = new() { "Check trellis anchors and end posts", "Tie down loose canopy growth" },
            [CropFamily.Berry] = new() { "Secure row covers and tunnel plastic" },
            [CropFamily.RowCrop] = new() { "Delay spraying until winds ease", "Check for lodging after the wind passes" },
        };

        private static readonly Dictionary<CropFamily, string> RainTips = new()
        {
            [CropFamily.TreeFruit] = "Standing water around roots can trigger root rot - make sure orchard drainage channels are clear.",
            [CropFamily.Vine] = "Wet canopy conditions raise mildew risk - improve airflow by removing excess leaf cover around fruit clusters if possible.",
            [CropFamily.Berry] = "Waterlogged beds encourage root rot - check that drainage furrows are clear before the rain arrives.",
            [CropFamily.RowCrop] = "Waterlogging can suffocate roots within a day or two - clear drainage furrows and avoid low-lying fields if replanting.",
        };
        private static readonly Dictionary<CropFamily, List<string>> RainActions = new()
        {
            [CropFamily.TreeFruit] = new() { "Clear drainage channels", "Hold off on additional irrigation" },
            [CropFamily.Vine] = new() { "Thin canopy near fruit clusters for airflow", "Watch for mildew after the rain" },
            [CropFamily.Berry] = new() { "Clear drainage furrows", "Watch for root rot in low spots" },
            [CropFamily.RowCrop] = new() { "Clear drainage furrows", "Delay fertilizer application until fields drain" },
        };

        private static readonly Dictionary<CropFamily, string> HeatTips = new()
        {
            [CropFamily.TreeFruit] = "Sun-exposed fruit can scald in extreme heat - make sure irrigation keeps up, especially on young or shallow-rooted trees.",
            [CropFamily.Vine] = "Heat stress can stall ripening or scald exposed clusters - keep the canopy watered and avoid removing shading leaves during the hottest days.",
            [CropFamily.Berry] = "Shallow roots make berries quick to wilt in heat - check soil moisture daily and irrigate in early morning or evening.",
            [CropFamily.RowCrop] = "Heat during flowering can reduce fruit/seed set - keep soil moisture consistent and irrigate during cooler hours to reduce evaporation loss.",
        };
        private static readonly Dictionary<CropFamily, List<string>> HeatActions = new()
        {
            [CropFamily.TreeFruit] = new() { "Increase irrigation frequency", "Avoid pruning that removes fruit-shading leaves" },
            [CropFamily.Vine] = new() { "Keep canopy watered", "Avoid heavy leaf removal near clusters" },
            [CropFamily.Berry] = new() { "Irrigate in early morning or evening", "Check soil moisture daily" },
            [CropFamily.RowCrop] = new() { "Irrigate during cooler hours", "Avoid heat-stressing crops during flowering if avoidable" },
        };

        private static readonly Dictionary<CropFamily, string> HumidityTips = new()
        {
            [CropFamily.TreeFruit] = "Conditions favor fungal diseases like scab and brown rot - improve airflow with light pruning and consider a preventive fungicide if disease has appeared before.",
            [CropFamily.Vine] = "Conditions favor powdery and downy mildew - open up the canopy around clusters and consider a preventive fungicide application.",
            [CropFamily.Berry] = "Conditions favor botrytis (grey mold) on fruit - improve airflow between plants and harvest ripe fruit promptly rather than letting it sit wet.",
            [CropFamily.RowCrop] = "Conditions favor blight and leaf spot diseases - avoid overhead watering and consider a preventive fungicide if disease has appeared before.",
        };
        private static readonly Dictionary<CropFamily, List<string>> HumidityActions = new()
        {
            [CropFamily.TreeFruit] = new() { "Light pruning for airflow", "Watch for scab/brown rot symptoms" },
            [CropFamily.Vine] = new() { "Open canopy around fruit clusters", "Watch for powdery/downy mildew" },
            [CropFamily.Berry] = new() { "Harvest ripe fruit promptly", "Improve spacing/airflow between plants" },
            [CropFamily.RowCrop] = new() { "Avoid overhead watering", "Scout for blight/leaf spot" },
        };
    }
}
