using LeafUpload.Core.Abstractions;
using LeafUpload.Web.Services;
using LeafUpload.Web.ViewModels;
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
        private readonly FarmAdvisoryService _farmAdvisoryService;

        public AdvisoryController(IFarmerRepository farmerRepository, FarmAdvisoryService farmAdvisoryService)
        {
            _farmerRepository = farmerRepository;
            _farmAdvisoryService = farmAdvisoryService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var farms = await _farmerRepository.GetFarmsByFarmerIdAsync(farmerId);

            var viewModel = new List<FarmAdvisoryViewModel>();
            foreach (var farm in farms)
            {
                var advisory = await _farmAdvisoryService.GetOrGenerateAdvisoryAsync(farm);
                viewModel.Add(new FarmAdvisoryViewModel { Farm = farm, Advisory = advisory });
            }

            return View(viewModel);
        }
    }
}
