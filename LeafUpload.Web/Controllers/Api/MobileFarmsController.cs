using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using LeafUpload.Web.Models.Mobile;
using LeafUpload.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace LeafUpload.Web.Controllers.Api
{
    [ApiController]
    [Route("api/mobile/farms")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MobileFarmsController : ControllerBase
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IWeatherService _weatherService;

        public MobileFarmsController(IFarmerRepository farmerRepository, IWeatherService weatherService)
        {
            _farmerRepository = farmerRepository;
            _weatherService = weatherService;
        }

        private Guid GetFarmerId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetFarms()
        {
            var farms = await _farmerRepository.GetFarmsByFarmerIdAsync(GetFarmerId());
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            return Ok(farms.Select(f => ToDto(f, culture)));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFarm(CreateMobileFarmRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PlaceName))
                return BadRequest(new { error = "PlaceName is required." });

            if (string.IsNullOrWhiteSpace(request.CropType))
                return BadRequest(new { error = "CropType is required." });

            var (farm, error) = await MobileFarmBuilder.BuildFarmAsync(
                _weatherService, request.PlaceName, request.CropType,
                request.Latitude, request.Longitude, request.ResolvedLocationName);
            if (farm == null)
                return BadRequest(new { error });

            farm.FarmerId = GetFarmerId();
            await _farmerRepository.AddFarmAsync(farm);

            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            return Ok(ToDto(farm, culture));
        }

        private static MobileFarmDto ToDto(Farm farm, string culture) => new()
        {
            Id = farm.Id,
            PlaceName = farm.PlaceName,
            ResolvedLocationName = farm.ResolvedLocationName,
            CropType = farm.CropType,
            CropIcon = CropTaxonomy.IconFor(farm.CropType),
            CropNameLocalized = CropTaxonomy.NameFor(farm.CropType, culture),
            Latitude = farm.Latitude,
            Longitude = farm.Longitude,
        };
    }
}
