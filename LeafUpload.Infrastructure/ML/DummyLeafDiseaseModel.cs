using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;

namespace LeafUpload.Infrastructure.ML
{
    public class DummyLeafDiseaseModel : ILeafDiseaseModel
    {
        public Task<DiagnosisResult> PredictAsync(LeafSample sample)
        {
            var result = new DiagnosisResult
            {
                LeafSampleId = sample.Id,
                PredictedDisease = "Healthy",
                Confidence = 0.99f,
                TreatmentAdvice = "No treatment required."
            };

            return Task.FromResult(result);
        }
    }
}
