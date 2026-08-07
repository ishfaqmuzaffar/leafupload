using LeafUpload.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace LeafUpload.Web.Controllers.Api
{
    // No auth needed - mirrors AccountController.ReverseGeocode for the mobile
    // "use my current location" flow on Register/Create Farm.
    [ApiController]
    [Route("api/mobile/geo")]
    public class MobileGeoController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public MobileGeoController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("reverse")]
        public async Task<IActionResult> Reverse(double lat, double lon)
        {
            var resolvedName = await _weatherService.ReverseGeocodeAsync(lat, lon);
            return Ok(new { resolvedName });
        }
    }
}
