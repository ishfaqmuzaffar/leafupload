using System;
using System.Collections.Generic;
using System.Globalization;

namespace LeafUpload.Core.Models
{
    // Where a crop sits in its growing cycle, independent of the day-to-day weather
    // forecast (e.g. cherry harvest ending in June vs. apple harvest running into October).
    public enum SeasonPhase
    {
        OffSeason,
        Growing,
        Ripening,
        Harvest,
        PostHarvest,
    }

    public readonly record struct SeasonWindow(SeasonPhase Phase, int StartMonth, int EndMonth)
    {
        public bool Contains(int month) =>
            StartMonth <= EndMonth
                ? month >= StartMonth && month <= EndMonth
                : month >= StartMonth || month <= EndMonth;
    }

    // Approximate month-range phenology per crop for Jammu & Kashmir's growing conditions.
    // Deliberately month-granularity, not exact dates - actual timing shifts with elevation,
    // variety, and year. Falls back to the RowCrop-generic ("Tomato") calendar for any crop
    // not listed here.
    public static class CropCalendar
    {
        private static readonly IReadOnlyDictionary<string, SeasonWindow[]> Windows = new Dictionary<string, SeasonWindow[]>
        {
            ["Apple"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 12, 2),
                new SeasonWindow(SeasonPhase.Growing, 3, 5),
                new SeasonWindow(SeasonPhase.Ripening, 6, 7),
                new SeasonWindow(SeasonPhase.Harvest, 8, 10),
                new SeasonWindow(SeasonPhase.PostHarvest, 11, 11),
            },
            ["Cherry"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 12, 2),
                new SeasonWindow(SeasonPhase.Growing, 3, 4),
                new SeasonWindow(SeasonPhase.Ripening, 5, 5),
                new SeasonWindow(SeasonPhase.Harvest, 6, 6),
                new SeasonWindow(SeasonPhase.PostHarvest, 7, 11),
            },
            ["Peach"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 12, 2),
                new SeasonWindow(SeasonPhase.Growing, 3, 5),
                new SeasonWindow(SeasonPhase.Ripening, 6, 6),
                new SeasonWindow(SeasonPhase.Harvest, 7, 8),
                new SeasonWindow(SeasonPhase.PostHarvest, 9, 11),
            },
            ["Orange"] = new[]
            {
                // Grown in the Jammu subtropical belt - evergreen, so no true leaf-off
                // dormancy the way temperate Kashmir tree fruit has.
                new SeasonWindow(SeasonPhase.Growing, 2, 6),
                new SeasonWindow(SeasonPhase.Ripening, 7, 9),
                new SeasonWindow(SeasonPhase.Harvest, 10, 1),
            },
            ["Grape"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 12, 2),
                new SeasonWindow(SeasonPhase.Growing, 3, 5),
                new SeasonWindow(SeasonPhase.Ripening, 6, 7),
                new SeasonWindow(SeasonPhase.Harvest, 8, 9),
                new SeasonWindow(SeasonPhase.PostHarvest, 10, 11),
            },
            ["Blueberry"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 12, 2),
                new SeasonWindow(SeasonPhase.Growing, 3, 3),
                new SeasonWindow(SeasonPhase.Ripening, 4, 5),
                new SeasonWindow(SeasonPhase.Harvest, 6, 7),
                new SeasonWindow(SeasonPhase.PostHarvest, 8, 11),
            },
            ["Raspberry"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 12, 2),
                new SeasonWindow(SeasonPhase.Growing, 3, 4),
                new SeasonWindow(SeasonPhase.Ripening, 5, 5),
                new SeasonWindow(SeasonPhase.Harvest, 6, 8),
                new SeasonWindow(SeasonPhase.PostHarvest, 9, 11),
            },
            ["Strawberry"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 12, 1),
                new SeasonWindow(SeasonPhase.Growing, 2, 3),
                new SeasonWindow(SeasonPhase.Ripening, 4, 4),
                new SeasonWindow(SeasonPhase.Harvest, 5, 6),
                new SeasonWindow(SeasonPhase.PostHarvest, 7, 11),
            },
            ["Corn (Maize)"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 11, 3),
                new SeasonWindow(SeasonPhase.Growing, 4, 6),
                new SeasonWindow(SeasonPhase.Ripening, 7, 8),
                new SeasonWindow(SeasonPhase.Harvest, 9, 10),
            },
            ["Soybean"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 11, 4),
                new SeasonWindow(SeasonPhase.Growing, 5, 7),
                new SeasonWindow(SeasonPhase.Ripening, 8, 8),
                new SeasonWindow(SeasonPhase.Harvest, 9, 10),
            },
            ["Potato"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 10, 3),
                new SeasonWindow(SeasonPhase.Growing, 4, 5),
                new SeasonWindow(SeasonPhase.Ripening, 6, 7),
                new SeasonWindow(SeasonPhase.Harvest, 8, 9),
            },
            ["Squash"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 11, 4),
                new SeasonWindow(SeasonPhase.Growing, 5, 6),
                new SeasonWindow(SeasonPhase.Ripening, 7, 7),
                new SeasonWindow(SeasonPhase.Harvest, 8, 10),
            },
            ["Tomato"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 10, 3),
                new SeasonWindow(SeasonPhase.Growing, 4, 5),
                new SeasonWindow(SeasonPhase.Ripening, 6, 6),
                new SeasonWindow(SeasonPhase.Harvest, 7, 9),
            },
            ["Pepper (Bell)"] = new[]
            {
                new SeasonWindow(SeasonPhase.OffSeason, 10, 3),
                new SeasonWindow(SeasonPhase.Growing, 4, 5),
                new SeasonWindow(SeasonPhase.Ripening, 6, 6),
                new SeasonWindow(SeasonPhase.Harvest, 7, 9),
            },
        };

        private static SeasonWindow[] WindowsFor(string cropType) =>
            Windows.TryGetValue(cropType, out var windows) ? windows : Windows["Tomato"];

        public static SeasonPhase GetPhase(string cropType, DateOnly date)
        {
            foreach (var window in WindowsFor(cropType))
            {
                if (window.Contains(date.Month))
                    return window.Phase;
            }
            return SeasonPhase.Growing;
        }

        // Used to tell a farmer checking during the off-season when planting/growth
        // typically starts again (e.g. "the next planting window typically opens around April").
        public static string NextGrowingMonthName(string cropType)
        {
            foreach (var window in WindowsFor(cropType))
            {
                if (window.Phase == SeasonPhase.Growing)
                    return CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(window.StartMonth);
            }
            return CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(4);
        }
    }
}
