using LeafUpload.Core.Models;
using LeafUpload.Core.Abstractions;
using LeafUpload_Infrastructure;   // <-- ADD THIS
using Microsoft.ML;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.ML
{
    public class LeafDiseaseModel : ILeafDiseaseModel
    {
        private readonly ITreatmentAdvisor _treatmentAdvisor;

        public LeafDiseaseModel(ITreatmentAdvisor treatmentAdvisor)
        {
            _treatmentAdvisor = treatmentAdvisor;
        }

        public async Task<DiagnosisResult> PredictAsync(LeafSample sample)
        {
            // Phone photos carry an EXIF orientation tag rather than storing pixels
            // upright; the model's decoder ignores that tag, so without correcting
            // for it here, sideways/upside-down phone photos get fed to the model
            // rotated - something it never saw during training on upright lab photos.
            byte[] imageBytes;
            using (var image = await Image.LoadAsync(sample.FilePath))
            {
                image.Mutate(x => x.AutoOrient());
                using var buffer = new MemoryStream();
                await image.SaveAsync(buffer, new JpegEncoder());
                imageBytes = buffer.ToArray();
            }

            var input = new MLModel1.ModelInput
            {
                ImageSource = imageBytes,
                Label = "Unknown" // placeholder
            };

            // Run prediction
            var output = MLModel1.Predict(input);

            // Find top prediction
            var bestPrediction = output.PredictedLabel;
            var confidence = output.Score.Max();

            var diagnosis = new DiagnosisResult
            {
                PredictedDisease = bestPrediction,
                Confidence = confidence
            };
            diagnosis.TreatmentAdvice = _treatmentAdvisor.GetAdvice(diagnosis);

            return diagnosis;
        }
    }
}
