using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;

namespace LeafUpload.Web.Services
{
    // Shared by MobileAuthController.Register and MobileFarmsController.Create - both
    // build a Farm from a place name (or device GPS coords) the same way AccountController
    // does for the web form.
    public static class MobileFarmBuilder
    {
        public static async Task<(Farm? Farm, string? Error)> BuildFarmAsync(
            IWeatherService weatherService,
            string placeName,
            string cropType,
            double? latitude,
            double? longitude,
            string? resolvedLocationName)
        {
            var trimmedPlaceName = placeName.Trim();
            double lat, lon;
            string? resolvedName;

            if (latitude.HasValue && longitude.HasValue)
            {
                lat = latitude.Value;
                lon = longitude.Value;
                resolvedName = resolvedLocationName ?? trimmedPlaceName;
            }
            else
            {
                var location = await weatherService.GeocodeAsync(trimmedPlaceName);
                if (location == null)
                    return (null, "Couldn't find that location - try adding a district or state, e.g. \"Anantnag, Jammu and Kashmir\".");

                lat = location.Latitude;
                lon = location.Longitude;
                resolvedName = location.ResolvedName;
            }

            return (new Farm
            {
                PlaceName = trimmedPlaceName,
                CropType = cropType,
                Latitude = lat,
                Longitude = lon,
                ResolvedLocationName = resolvedName,
            }, null);
        }
    }
}
