namespace LeafUpload.Core.Models
{
    public class DiagnosisResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Link to the leaf sample
        public Guid LeafSampleId { get; set; }

        // Predicted disease name
        public string PredictedDisease { get; set; } = string.Empty;

        // Confidence score
        public float Confidence { get; set; }

        // Suggested treatment
        public string TreatmentAdvice { get; set; } = string.Empty;
    }
}
