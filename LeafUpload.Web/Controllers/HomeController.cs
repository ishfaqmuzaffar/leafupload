using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using LeafUpload.Web.Localization;
using LeafUpload.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeafUpload.Web.Controllers
{
    public class HomeController : Controller
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp", "image/bmp"
        };

        private readonly ILeafDiseaseModel _diseaseModel;
        private readonly IFarmerRepository _farmerRepository;
        private readonly IAdvisoryRepository _advisoryRepository;

        public HomeController(ILeafDiseaseModel diseaseModel, IFarmerRepository farmerRepository, IAdvisoryRepository advisoryRepository)
        {
            _diseaseModel = diseaseModel;
            _farmerRepository = farmerRepository;
            _advisoryRepository = advisoryRepository;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Landing()
        {
            var farms = await _farmerRepository.GetAllFarmsAsync();
            var latestAdvisories = await _advisoryRepository.GetLatestAdvisoriesForAllFarmsAsync();

            var alertCounts = latestAdvisories
                .SelectMany(a => a.Alerts)
                .Where(a => a.Severity != AdvisorySeverity.Info)
                .GroupBy(a => a.Title)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count());

            var model = new LandingViewModel
            {
                FarmerCount = await _farmerRepository.GetFarmerCountAsync(),
                FarmCount = farms.Count,
                CropVarietyCount = CropTaxonomy.Crops.Count,
                ActiveAlertCount = alertCounts.Values.Sum(),
                CropCounts = farms
                    .GroupBy(f => f.CropType)
                    .OrderByDescending(g => g.Count())
                    .ToDictionary(g => g.Key, g => g.Count()),
                AlertCounts = alertCounts,
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = T.S("diag.errorInvalidImage");
                return View();
            }

            if (file.Length > MaxFileSizeBytes)
            {
                ViewBag.Error = T.S("diag.errorTooLarge");
                return View();
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                ViewBag.Error = T.S("diag.errorUnsupportedType");
                return View();
            }

            // Save temp file
            var filePath = Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Run prediction
                var sample = new LeafUpload.Core.Models.LeafSample
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    UploadedAt = System.DateTime.UtcNow
                };

                var diagnosis = await _diseaseModel.PredictAsync(sample);

                ViewBag.Disease = diagnosis.PredictedDisease;
                ViewBag.Confidence = diagnosis.Confidence;
                ViewBag.Treatment = diagnosis.TreatmentAdvice;
                ViewBag.Symptoms = diagnosis.Symptoms;

                return View();
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }
    }
}
