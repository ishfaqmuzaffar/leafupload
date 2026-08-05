using LeafUpload.Core.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeafUpload.Web.Controllers
{
    [Authorize]
    [Route("Advisory")]
    public class AdvisoryController : Controller
    {
        private readonly IFarmerRepository _farmerRepository;

        public AdvisoryController(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var farms = await _farmerRepository.GetFarmsByFarmerIdAsync(farmerId);
            return View(farms);
        }
    }
}
