using LeafUpload.Core.Models;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IAdvisoryGenerator
    {
        Task<string> GenerateAdvisoryAsync(string cropType, string placeName, WeatherForecast forecast);
    }
}
