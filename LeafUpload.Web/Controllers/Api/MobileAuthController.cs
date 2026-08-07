using LeafUpload.Core.Abstractions;
using LeafUpload.Web.Models.Mobile;
using LeafUpload.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeafUpload.Web.Controllers.Api
{
    // JSON auth for the mobile app - issues JWTs. The web app keeps using
    // AccountController's cookie auth; this doesn't touch that flow.
    [ApiController]
    [Route("api/mobile/auth")]
    public class MobileAuthController : ControllerBase
    {
        private readonly IFarmerAuthService _authService;
        private readonly IWeatherService _weatherService;
        private readonly IJwtTokenService _jwtTokenService;

        public MobileAuthController(
            IFarmerAuthService authService,
            IWeatherService weatherService,
            IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _weatherService = weatherService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(MobileRegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { error = "Username and password are required." });

            if (string.IsNullOrWhiteSpace(request.PlaceName))
                return BadRequest(new { error = "PlaceName is required." });

            if (string.IsNullOrWhiteSpace(request.CropType))
                return BadRequest(new { error = "CropType is required." });

            var (farm, error) = await MobileFarmBuilder.BuildFarmAsync(
                _weatherService, request.PlaceName, request.CropType,
                request.Latitude, request.Longitude, request.ResolvedLocationName);
            if (farm == null)
                return BadRequest(new { error });

            var result = await _authService.RegisterAsync(request.Username, request.Password, farm);
            if (!result.Succeeded)
                return BadRequest(new { error = result.ErrorMessage ?? "Registration failed." });

            var token = _jwtTokenService.IssueToken(result.Farmer!);
            return Ok(new MobileAuthResponse
            {
                Token = token,
                FarmerId = result.Farmer!.Id,
                Username = result.Farmer.Username,
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(MobileLoginRequest request)
        {
            var farmer = await _authService.ValidateCredentialsAsync(request.Username, request.Password);
            if (farmer == null)
                return Unauthorized(new { error = "Invalid username or password." });

            var token = _jwtTokenService.IssueToken(farmer);
            return Ok(new MobileAuthResponse
            {
                Token = token,
                FarmerId = farmer.Id,
                Username = farmer.Username,
            });
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Me()
        {
            var farmerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { FarmerId = farmerId, Username = username });
        }
    }
}
