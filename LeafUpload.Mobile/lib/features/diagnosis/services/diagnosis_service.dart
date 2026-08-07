import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import 'package:http_parser/http_parser.dart';
import 'package:image_picker/image_picker.dart';
import 'package:krishimitra_mobile/core/config/api_config.dart';
import 'package:krishimitra_mobile/features/diagnosis/models/diagnosis_result_model.dart';

class DiagnosisService {
  // DiagnosisController validates the multipart file's content type strictly
  // (jpeg/png/webp/bmp only) - http.MultipartFile defaults to
  // application/octet-stream unless we set this explicitly, which the server rejects.
  static MediaType _mediaTypeFor(String filename) {
    final lower = filename.toLowerCase();
    if (lower.endsWith('.png')) return MediaType('image', 'png');
    if (lower.endsWith('.webp')) return MediaType('image', 'webp');
    if (lower.endsWith('.bmp')) return MediaType('image', 'bmp');
    return MediaType('image', 'jpeg');
  }

  // POST /api/Diagnosis/upload needs no auth - it's a standalone image ->
  // diagnosis endpoint, not tied to a farmer or farm.
  Future<DiagnosisResultModel> diagnose(XFile image) async {
    final request = http.MultipartRequest('POST', Uri.parse(ApiConfig.diagnosisUpload));
    final filename = image.name.isNotEmpty ? image.name : 'leaf.jpg';
    final mediaType = _mediaTypeFor(filename);

    if (kIsWeb) {
      final bytes = await image.readAsBytes();
      request.files.add(http.MultipartFile.fromBytes('file', bytes, filename: filename, contentType: mediaType));
    } else {
      request.files.add(await http.MultipartFile.fromPath('file', image.path, filename: filename, contentType: mediaType));
    }

    final streamedResponse = await request.send();
    final body = await streamedResponse.stream.bytesToString();

    if (streamedResponse.statusCode < 200 || streamedResponse.statusCode >= 300) {
      final decoded = jsonDecode(body) as Map<String, dynamic>;
      throw Exception(decoded['error']?.toString() ?? 'Diagnosis failed.');
    }

    return DiagnosisResultModel.fromJson(jsonDecode(body) as Map<String, dynamic>);
  }
}
