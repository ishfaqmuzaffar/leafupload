using LeafUpload.Core.Abstractions;
using LeafUpload.Web.Models.Mobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeafUpload.Web.Controllers.Api
{
    [ApiController]
    [Route("api/mobile/devices")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MobileDevicesController : ControllerBase
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;

        public MobileDevicesController(IDeviceTokenRepository deviceTokenRepository)
        {
            _deviceTokenRepository = deviceTokenRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDeviceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { error = "Token is required." });

            var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _deviceTokenRepository.UpsertAsync(farmerId, request.Token, request.Platform);
            return Ok(new { success = true });
        }
    }
}
