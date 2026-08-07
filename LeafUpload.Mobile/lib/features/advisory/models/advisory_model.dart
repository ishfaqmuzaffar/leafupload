// Mirrors LeafUpload.Core/Models/AdvisoryAlert.cs - severity is serialized as the
// enum's int value: Info=0, Caution=1, Warning=2, Critical=3.
enum AdvisorySeverity { info, caution, warning, critical }

AdvisorySeverity _severityFromInt(dynamic value) {
  final index = value is int ? value : int.tryParse(value?.toString() ?? '') ?? 0;
  if (index >= 0 && index < AdvisorySeverity.values.length) {
    return AdvisorySeverity.values[index];
  }
  return AdvisorySeverity.info;
}

class AdvisoryAlertModel {
  final String icon;
  final String title;
  final AdvisorySeverity severity;
  final String message;
  final List<String> actions;
  final String? timing;

  AdvisoryAlertModel({
    required this.icon,
    required this.title,
    required this.severity,
    required this.message,
    required this.actions,
    required this.timing,
  });

  factory AdvisoryAlertModel.fromJson(Map<String, dynamic> json) {
    return AdvisoryAlertModel(
      icon: json['icon']?.toString() ?? 'ℹ️',
      title: json['title']?.toString() ?? '',
      severity: _severityFromInt(json['severity']),
      message: json['message']?.toString() ?? '',
      actions: (json['actions'] as List<dynamic>? ?? []).map((a) => a.toString()).toList(),
      timing: json['timing']?.toString(),
    );
  }
}

// Mirrors LeafUpload.Core/Models/WeatherForecast.cs - parallel arrays, one entry per day.
class WeatherForecastModel {
  final List<String> dates;
  final List<double> tempMaxC;
  final List<double> tempMinC;
  final List<double> precipitationMm;
  final List<int?> weatherCode;

  WeatherForecastModel({
    required this.dates,
    required this.tempMaxC,
    required this.tempMinC,
    required this.precipitationMm,
    required this.weatherCode,
  });

  int get dayCount => dates.length;

  factory WeatherForecastModel.fromJson(Map<String, dynamic> json) {
    List<double> readDoubles(String key) =>
        (json[key] as List<dynamic>? ?? []).map((v) => (v as num).toDouble()).toList();

    return WeatherForecastModel(
      dates: (json['dates'] as List<dynamic>? ?? []).map((d) => d.toString()).toList(),
      tempMaxC: readDoubles('tempMaxC'),
      tempMinC: readDoubles('tempMinC'),
      precipitationMm: readDoubles('precipitationMm'),
      weatherCode: (json['weatherCode'] as List<dynamic>?)?.map((c) => c as int?).toList() ?? [],
    );
  }
}

class AdvisoryModel {
  final String farmId;
  final String? summary;
  final List<AdvisoryAlertModel> alerts;
  final WeatherForecastModel? forecast;
  final DateTime? generatedAt;

  AdvisoryModel({
    required this.farmId,
    required this.summary,
    required this.alerts,
    required this.forecast,
    required this.generatedAt,
  });

  factory AdvisoryModel.fromJson(Map<String, dynamic> json) {
    return AdvisoryModel(
      farmId: json['farmId']?.toString() ?? '',
      summary: json['summary']?.toString(),
      alerts: (json['alerts'] as List<dynamic>? ?? [])
          .map((a) => AdvisoryAlertModel.fromJson(a as Map<String, dynamic>))
          .toList(),
      forecast: json['forecast'] == null ? null : WeatherForecastModel.fromJson(json['forecast'] as Map<String, dynamic>),
      generatedAt: json['generatedAt'] == null ? null : DateTime.tryParse(json['generatedAt'].toString()),
    );
  }
}
