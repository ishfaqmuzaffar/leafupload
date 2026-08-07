using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using System.Globalization;

namespace LeafUpload.Infrastructure.Rules
{
    public class SimpleTreatmentAdvisor : ITreatmentAdvisor
    {
        public DiseaseInfo GetAdvice(DiagnosisResult diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis.PredictedDisease))
            {
                return new DiseaseInfo
                {
                    Treatment = "No disease detected. Keep monitoring your crops and maintain good irrigation.",
                };
            }

            // Set by RequestLocalizationMiddleware from the farmer's chosen language
            // cookie - flows through the async call chain automatically, so no need
            // to thread a culture parameter down from the controller.
            var cultureCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var info = DiseaseKnowledgeBase.Lookup(diagnosis.PredictedDisease, cultureCode);

            return info ?? new DiseaseInfo { Treatment = "Open the KKG app to chat or video call with an expert about treatment." };
        }
    }
}
