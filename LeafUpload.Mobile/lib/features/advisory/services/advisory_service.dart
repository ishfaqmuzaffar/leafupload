import 'dart:convert';
import 'package:krishimitra_mobile/core/config/api_config.dart';
import 'package:krishimitra_mobile/core/network/auth_http.dart';
import 'package:krishimitra_mobile/features/advisory/models/advisory_model.dart';

class AdvisoryService {
  final AuthHttp _authHttp = AuthHttp();

  Future<AdvisoryModel> getAdvisoryForFarm(String farmId) async {
    final response = await _authHttp.get(Uri.parse(ApiConfig.advisoryForFarm(farmId)));

    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw Exception('Failed to fetch advisory');
    }

    return AdvisoryModel.fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }
}
