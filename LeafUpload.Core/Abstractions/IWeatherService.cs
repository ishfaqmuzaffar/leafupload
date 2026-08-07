using LeafUpload.Core.Models;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IWeatherService
    {
        Task<GeoLocation?> GeocodeAsync(string placeName);
        Task<WeatherForecast> GetForecastAsync(double latitude, double longitude);

        // Coordinates -> human-readable place name, for the "use my location" flow
        // where the farmer never typed a place name to begin with. Null if no
        // address could be resolved for the given point.
        Task<string?> ReverseGeocodeAsync(double latitude, double longitude);
    }
}
