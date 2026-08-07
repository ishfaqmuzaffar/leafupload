class ApiConfig {
  // Points at the deployed backend so it works from a real device/emulator
  // (which can't reach the dev machine's "localhost"). Swap to
  // http://10.0.2.2:5066 to hit a local dev server from the Android emulator.
  static const String baseUrl = 'https://farmer.jkcip.in';

  static String get register => '$baseUrl/api/mobile/auth/register';
  static String get login => '$baseUrl/api/mobile/auth/login';
  static String get me => '$baseUrl/api/mobile/auth/me';

  static String get crops => '$baseUrl/api/mobile/crops';
  static String reverseGeocode(double lat, double lon) => '$baseUrl/api/mobile/geo/reverse?lat=$lat&lon=$lon';
  static String get farms => '$baseUrl/api/mobile/farms';
  static String get registerDevice => '$baseUrl/api/mobile/devices/register';
  static String advisoryForFarm(String farmId) => '$baseUrl/api/mobile/advisory/farms/$farmId';

  static String get diagnosisUpload => '$baseUrl/api/Diagnosis/upload';
}
