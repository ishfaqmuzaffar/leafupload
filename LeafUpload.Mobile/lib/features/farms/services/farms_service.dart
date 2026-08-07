import 'dart:convert';
import 'package:krishimitra_mobile/core/config/api_config.dart';
import 'package:krishimitra_mobile/core/network/auth_http.dart';
import 'package:krishimitra_mobile/features/farms/models/farm_model.dart';

class FarmsService {
  final AuthHttp _authHttp = AuthHttp();

  Future<List<FarmModel>> getFarms() async {
    final response = await _authHttp.get(Uri.parse(ApiConfig.farms));

    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw Exception('Failed to fetch farms');
    }

    final data = jsonDecode(response.body) as List<dynamic>;
    return data.map((item) => FarmModel.fromJson(item as Map<String, dynamic>)).toList();
  }

  Future<String?> createFarm({
    required String placeName,
    required String cropType,
    double? latitude,
    double? longitude,
  }) async {
    final response = await _authHttp.post(
      Uri.parse(ApiConfig.farms),
      body: {
        'placeName': placeName,
        'cropType': cropType,
        if (latitude != null) 'latitude': latitude,
        if (longitude != null) 'longitude': longitude,
      },
    );

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return null;
    }

    final body = jsonDecode(response.body) as Map<String, dynamic>;
    return body['error']?.toString() ?? 'Failed to create farm.';
  }
}
