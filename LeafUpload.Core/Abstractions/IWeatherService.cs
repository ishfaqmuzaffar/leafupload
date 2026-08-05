using LeafUpload.Core.Models;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IWeatherService
    {
        Task<GeoLocation?> GeocodeAsync(string placeName);
        Task<WeatherForecast> GetForecastAsync(double latitude, double longitude);
    }
}
