using LeafUpload.Core.Models;
using LeafUpload.Core.Abstractions;
using LeafUpload_Infrastructure;   // <-- ADD THIS
using Microsoft.ML;
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
            // Load the image into ModelInput
            var imageBytes = await File.ReadAllBytesAsync(sample.FilePath);

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
