import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:krishimitra_mobile/core/config/api_config.dart';
import 'package:krishimitra_mobile/core/storage/token_storage.dart';
import 'package:krishimitra_mobile/features/auth/models/auth_response_model.dart';
import 'package:krishimitra_mobile/features/auth/models/current_user_model.dart';

class AuthService {
  final TokenStorage _tokenStorage = TokenStorage();

  Future<AuthResponseModel> login({
    required String username,
    required String password,
  }) async {
    final response = await http.post(
      Uri.parse(ApiConfig.login),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'username': username,
        'password': password,
      }),
    );

    final body = jsonDecode(response.body) as Map<String, dynamic>;

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return AuthResponseModel.fromJson(body);
    }

    return AuthResponseModel(
      success: false,
      message: body['error']?.toString() ?? 'Login failed.',
    );
  }

  Future<AuthResponseModel> register({
    required String username,
    required String password,
    required String placeName,
    required String cropType,
    double? latitude,
    double? longitude,
  }) async {
    final response = await http.post(
      Uri.parse(ApiConfig.register),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'username': username,
        'password': password,
        'placeName': placeName,
        'cropType': cropType,
        if (latitude != null) 'latitude': latitude,
        if (longitude != null) 'longitude': longitude,
      }),
    );

    final body = jsonDecode(response.body) as Map<String, dynamic>;

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return AuthResponseModel.fromJson(body);
    }

    return AuthResponseModel(
      success: false,
      message: body['error']?.toString() ?? 'Registration failed.',
    );
  }

  Future<CurrentUserModel?> getCurrentUser() async {
    final token = await _tokenStorage.getToken();

    if (token == null || token.isEmpty) {
      return null;
    }

    final response = await http.get(
      Uri.parse(ApiConfig.me),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
    );

    if (response.statusCode >= 200 && response.statusCode < 300) {
      final body = jsonDecode(response.body) as Map<String, dynamic>;
      return CurrentUserModel.fromJson(body);
    }

    return null;
  }

  Future<void> logout() async {
    await _tokenStorage.clearToken();
  }
}
