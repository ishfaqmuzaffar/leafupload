// Matches MobileFarmDto from api/mobile/farms.
class FarmModel {
  final String id;
  final String placeName;
  final String? resolvedLocationName;
  final String cropType;
  final String cropIcon;
  final String cropNameLocalized;
  final double? latitude;
  final double? longitude;

  FarmModel({
    required this.id,
    required this.placeName,
    required this.resolvedLocationName,
    required this.cropType,
    required this.cropIcon,
    required this.cropNameLocalized,
    required this.latitude,
    required this.longitude,
  });

  factory FarmModel.fromJson(Map<String, dynamic> json) {
    return FarmModel(
      id: json['id']?.toString() ?? '',
      placeName: json['placeName']?.toString() ?? '',
      resolvedLocationName: json['resolvedLocationName']?.toString(),
      cropType: json['cropType']?.toString() ?? '',
      cropIcon: json['cropIcon']?.toString() ?? '',
      cropNameLocalized: json['cropNameLocalized']?.toString() ?? '',
      latitude: (json['latitude'] as num?)?.toDouble(),
      longitude: (json['longitude'] as num?)?.toDouble(),
    );
  }
}
