import 'package:krishimitra_mobile/core/config/api_config.dart';
import 'package:krishimitra_mobile/core/network/auth_http.dart';

class DevicesService {
  final AuthHttp _authHttp = AuthHttp();

  Future<void> registerToken(String token, String platform) async {
    await _authHttp.post(
      Uri.parse(ApiConfig.registerDevice),
      body: {'token': token, 'platform': platform},
    );
  }
}
