import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:krishimitra_mobile/core/config/api_config.dart';

class GeoService {
  // No auth needed - GET /api/mobile/geo/reverse is public.
  Future<String?> reverseGeocode(double lat, double lon) async {
    final response = await http.get(Uri.parse(ApiConfig.reverseGeocode(lat, lon)));

    if (response.statusCode < 200 || response.statusCode >= 300) {
      return null;
    }

    final body = jsonDecode(response.body) as Map<String, dynamic>;
    return body['resolvedName']?.toString();
  }
}
