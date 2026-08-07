import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:krishimitra_mobile/core/storage/token_storage.dart';

class AuthHttp {
  final TokenStorage _tokenStorage = TokenStorage();

  Future<Map<String, String>> _headers() async {
    final token = await _tokenStorage.getToken();

    return {
      'Content-Type': 'application/json',
      if (token != null && token.isNotEmpty) 'Authorization': 'Bearer $token',
    };
  }

  Future<http.Response> get(Uri uri) async {
    return http.get(uri, headers: await _headers());
  }

  Future<http.Response> post(Uri uri, {Object? body}) async {
    return http.post(
      uri,
      headers: await _headers(),
      body: body == null ? null : jsonEncode(body),
    );
  }
}