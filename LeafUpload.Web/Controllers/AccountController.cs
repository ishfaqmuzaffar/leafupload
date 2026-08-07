using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using LeafUpload.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeafUpload.Web.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IFarmerAuthService _authService;
        private readonly IWeatherService _weatherService;

        public AccountController(IFarmerAuthService authService, IWeatherService weatherService)
        {
            _authService = authService;
            _weatherService = weatherService;
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            ViewBag.Crops = CropTaxonomy.Crops;
            return View(new RegisterViewModel());
        }

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Crops = CropTaxonomy.Crops;

            if (!ModelState.IsValid)
                return View(model);

            var placeName = model.PlaceName.Trim();
            double latitude, longitude;
            string resolvedName;

            if (model.Latitude.HasValue && model.Longitude.HasValue)
            {
                // Farmer located (and optionally fine-tuned) their farm via the map pin.
                latitude = model.Latitude.Value;
                longitude = model.Longitude.Value;
                resolvedName = model.ResolvedLocationName ?? placeName;
            }
            else
            {
                // Map wasn't used (e.g. JS disabled) - fall back to geocoding the place name alone.
                var location = await _weatherService.GeocodeAsync(placeName);
                if (location == null)
                {
                    ModelState.AddModelError(nameof(model.PlaceName),
                        "Couldn't find that location - try adding a district or state, e.g. \"Anantnag, Jammu and Kashmir\".");
                    return View(model);
                }
                latitude = location.Latitude;
                longitude = location.Longitude;
                resolvedName = location.ResolvedName;
            }

            var farm = new Farm
            {
                PlaceName = placeName,
                CropType = model.CropType,
                Latitude = latitude,
                Longitude = longitude,
                ResolvedLocationName = resolvedName,
            };

            var result = await _authService.RegisterAsync(model.Username, model.Password, farm);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Registration failed.");
                return View(model);
            }

            await SignInFarmerAsync(result.Farmer!);
            return RedirectToAction("Index", "Advisory");
        }

        [HttpGet("GeocodePreview")]
        public async Task<IActionResult> GeocodePreview(string? placeName)
        {
            if (string.IsNullOrWhiteSpace(placeName))
                return Json(new { found = false });

            var location = await _weatherService.GeocodeAsync(placeName.Trim());
            if (location == null)
                return Json(new { found = false });

            return Json(new { found = true, latitude = location.Latitude, longitude = location.Longitude, resolvedName = location.ResolvedName });
        }

        [HttpGet("ReverseGeocode")]
        public async Task<IActionResult> ReverseGeocode(double lat, double lon)
        {
            var resolvedName = await _weatherService.ReverseGeocodeAsync(lat, lon);
            return Json(new { resolvedName });
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var farmer = await _authService.ValidateCredentialsAsync(model.Username, model.Password);
            if (farmer == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            await SignInFarmerAsync(farmer);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Advisory");
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInFarmerAsync(Farmer farmer)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, farmer.Id.ToString()),
                new(ClaimTypes.Name, farmer.Username),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
