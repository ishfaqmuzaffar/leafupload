using LeafUpload.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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

        public HomeController(ILeafDiseaseModel diseaseModel)
        {
            _diseaseModel = diseaseModel;
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
                ViewBag.Error = "Please upload a valid image.";
                return View();
            }

            if (file.Length > MaxFileSizeBytes)
            {
                ViewBag.Error = "File is too large. Maximum size is 10 MB.";
                return View();
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                ViewBag.Error = "Unsupported file type. Please upload a JPEG, PNG, WEBP, or BMP image.";
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
