// Matches DiagnosisController's upload response shape exactly.
class DiagnosisResultModel {
  final String sampleId;
  final String diagnosisId;
  final String disease;
  final double confidence;
  final String treatment;
  final List<String> symptoms;

  DiagnosisResultModel({
    required this.sampleId,
    required this.diagnosisId,
    required this.disease,
    required this.confidence,
    required this.treatment,
    required this.symptoms,
  });

  factory DiagnosisResultModel.fromJson(Map<String, dynamic> json) {
    return DiagnosisResultModel(
      sampleId: json['sampleId']?.toString() ?? '',
      diagnosisId: json['diagnosisId']?.toString() ?? '',
      disease: json['disease']?.toString() ?? 'Unknown',
      confidence: (json['confidence'] as num?)?.toDouble() ?? 0,
      treatment: json['treatment']?.toString() ?? '',
      symptoms: (json['symptoms'] as List<dynamic>? ?? []).map((s) => s.toString()).toList(),
    );
  }

  // Raw ML labels look like "Apple___Apple_scab" or "Tomato___healthy" -
  // turn that into "Apple scab" / "Healthy" for display.
  String get displayName {
    final afterCrop = disease.contains('___') ? disease.split('___').last : disease;
    final spaced = afterCrop.replaceAll('_', ' ').trim();
    if (spaced.isEmpty) return disease;
    return spaced[0].toUpperCase() + spaced.substring(1);
  }

  bool get isHealthy => disease.toLowerCase().contains('healthy');
}
