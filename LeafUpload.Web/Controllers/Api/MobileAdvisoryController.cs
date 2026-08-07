using LeafUpload.Core.Abstractions;
using LeafUpload.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeafUpload.Web.Controllers.Api
{
    [ApiController]
    [Route("api/mobile/advisory")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MobileAdvisoryController : ControllerBase
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly FarmAdvisoryService _farmAdvisoryService;

        public MobileAdvisoryController(IFarmerRepository farmerRepository, FarmAdvisoryService farmAdvisoryService)
        {
            _farmerRepository = farmerRepository;
            _farmAdvisoryService = farmAdvisoryService;
        }

        [HttpGet("farms/{farmId:guid}")]
        public async Task<IActionResult> GetForFarm(Guid farmId)
        {
            var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var farms = await _farmerRepository.GetFarmsByFarmerIdAsync(farmerId);
            var farm = farms.FirstOrDefault(f => f.Id == farmId);
            if (farm == null)
                return NotFound(new { error = "Farm not found." });

            var advisory = await _farmAdvisoryService.GetOrGenerateAdvisoryAsync(farm);
            if (advisory == null)
                return Ok(new { FarmId = farmId, Summary = (string?)null, Alerts = Array.Empty<object>(), Forecast = (object?)null, GeneratedAt = (DateTime?)null });

            return Ok(new
            {
                FarmId = farmId,
                Summary = advisory.AdvisoryText,
                advisory.Alerts,
                advisory.Forecast,
                advisory.GeneratedAt,
            });
        }
    }
}
