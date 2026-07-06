using LeafUpload.Core.Models;

namespace LeafUpload.Core.Abstractions
{
    public interface ITreatmentAdvisor
    {
        string GetAdvice(DiagnosisResult diagnosis);
    }
}
