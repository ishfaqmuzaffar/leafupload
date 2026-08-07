import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:krishimitra_mobile/core/config/api_config.dart';
import 'package:krishimitra_mobile/features/crops/models/crop_model.dart';

class CropsService {
  // No auth needed - GET /api/mobile/crops is a public endpoint.
  Future<List<CropModel>> getCrops() async {
    final response = await http.get(Uri.parse(ApiConfig.crops));

    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw Exception('Failed to fetch crops');
    }

    final data = jsonDecode(response.body) as List<dynamic>;
    return data.map((item) => CropModel.fromJson(item as Map<String, dynamic>)).toList();
  }
}
