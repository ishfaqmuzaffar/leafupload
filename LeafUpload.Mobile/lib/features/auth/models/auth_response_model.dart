class AuthResponseModel {
  final bool success;
  final String message;
  final String? token;
  final String? farmerId;
  final String? username;

  AuthResponseModel({
    required this.success,
    required this.message,
    this.token,
    this.farmerId,
    this.username,
  });

  // MobileAuthController returns a flat {token, farmerId, username} body on
  // success, or {error} on failure - no success/message/data envelope.
  factory AuthResponseModel.fromJson(Map<String, dynamic> json) {
    return AuthResponseModel(
      success: json['token'] != null,
      message: json['error']?.toString() ?? '',
      token: json['token']?.toString(),
      farmerId: json['farmerId']?.toString(),
      username: json['username']?.toString(),
    );
  }
}
