using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;

namespace LeafUpload.Infrastructure.Rules
{
    public class SimpleTreatmentAdvisor : ITreatmentAdvisor
    {
        public string GetAdvice(DiagnosisResult diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis.PredictedDisease))
                return "No disease detected. Keep monitoring your crops and maintain good irrigation.";

            return diagnosis.PredictedDisease switch
            {
                "Blight" => "Apply fungicides and remove infected leaves.",
                "Rust" => "Use resistant varieties and fungicidal sprays.",
                "Mildew" => "Ensure airflow between plants and apply fungicides.",
                _ => "Consult a local agronomist for treatment."
            };
        }
    }
}
