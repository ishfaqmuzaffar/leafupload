// Matches GET /api/mobile/crops - the same fixed 14-crop list the web app's
// Register.cshtml dropdown uses (CropTaxonomy.Crops), used by both the
// registration screen and the "add a farm" flow.
class CropModel {
  final String name;
  final String icon;
  final String family;
  final String localizedName;

  CropModel({
    required this.name,
    required this.icon,
    required this.family,
    required this.localizedName,
  });

  factory CropModel.fromJson(Map<String, dynamic> json) {
    return CropModel(
      name: json['name']?.toString() ?? '',
      icon: json['icon']?.toString() ?? '',
      family: json['family']?.toString() ?? '',
      localizedName: json['localizedName']?.toString() ?? '',
    );
  }
}
