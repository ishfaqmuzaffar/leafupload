namespace LeafUpload.Core.Models
{
    // WMO weather codes used by Open-Meteo (https://open-meteo.com/en/docs), rendered
    // for the farmer-facing 7-day strip on the Advisory page.
    public static class WeatherCodeDisplay
    {
        public static string IconFor(int? code) => code switch
        {
            0 => "☀️",
            1 or 2 or 3 => "⛅",
            45 or 48 => "🌫️",
            51 or 53 or 55 or 56 or 57 => "🌦️",
            61 or 63 or 65 or 66 or 67 => "🌧️",
            71 or 73 or 75 or 77 => "🌨️",
            80 or 81 or 82 => "🌦️",
            85 or 86 => "🌨️",
            95 => "⛈️",
            96 or 99 => "🧊",
            _ => "🌤️",
        };

        public static string LabelFor(int? code) => code switch
        {
            0 => "Clear",
            1 or 2 or 3 => "Partly cloudy",
            45 or 48 => "Fog",
            51 or 53 or 55 => "Drizzle",
            56 or 57 => "Freezing drizzle",
            61 or 63 or 65 => "Rain",
            66 or 67 => "Freezing rain",
            71 or 73 or 75 or 77 => "Snow",
            80 or 81 or 82 => "Rain showers",
            85 or 86 => "Snow showers",
            95 => "Thunderstorm",
            96 or 99 => "Hail",
            _ => "Unsettled",
        };
    }
}
