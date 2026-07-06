using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeafUpload.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosisController : ControllerBase
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp", "image/bmp"
        };

        private readonly ILeafRepository _repository;
        private readonly ILeafDiseaseModel _diseaseModel;

        public DiagnosisController(ILeafRepository repository, ILeafDiseaseModel diseaseModel)
        {
            _repository = repository;
            _diseaseModel = diseaseModel;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // 👈 tell Swagger this expects form-data
        public async Task<IActionResult> UploadLeaf([FromForm] UploadLeafRequest request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > MaxFileSizeBytes)
                return BadRequest("File is too large. Maximum size is 10 MB.");

            if (!AllowedContentTypes.Contains(file.ContentType))
                return BadRequest("Unsupported file type. Please upload a JPEG, PNG, WEBP, or BMP image.");

            var filePath = Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var sample = new LeafSample
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                await _repository.SaveSampleAsync(sample);

                var diagnosis = await _diseaseModel.PredictAsync(sample);
                diagnosis.LeafSampleId = sample.Id;

                await _repository.SaveDiagnosisAsync(diagnosis);

                return Ok(new
                {
                    SampleId = sample.Id,
                    DiagnosisId = diagnosis.Id,
                    Disease = diagnosis.PredictedDisease,
                    Confidence = diagnosis.Confidence,
                    Treatment = diagnosis.TreatmentAdvice
                });
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }
    }

    // DTO for file upload
    public class UploadLeafRequest
    {
        [FromForm(Name = "file")]
        public required IFormFile File { get; set; }
    }
}
