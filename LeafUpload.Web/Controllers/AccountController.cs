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

        public AccountController(IFarmerAuthService authService)
        {
            _authService = authService;
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

            var farm = new Farm
            {
                PlaceName = model.PlaceName.Trim(),
                CropType = model.CropType,
                // Geocoding (Latitude/Longitude/ResolvedLocationName) is wired up in a
                // later phase; the farm is saved with just the raw place name for now.
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
