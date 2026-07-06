using LeafUpload.Core.Models;

namespace LeafUpload.Core.Abstractions
{
    public interface ILeafDiseaseModel
    {
        Task<DiagnosisResult> PredictAsync(LeafSample sample);
    }
}
