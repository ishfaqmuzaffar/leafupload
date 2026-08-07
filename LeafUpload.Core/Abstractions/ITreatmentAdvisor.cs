using LeafUpload.Core.Models;

namespace LeafUpload.Core.Abstractions
{
    public interface ITreatmentAdvisor
    {
        DiseaseInfo GetAdvice(DiagnosisResult diagnosis);
    }
}
