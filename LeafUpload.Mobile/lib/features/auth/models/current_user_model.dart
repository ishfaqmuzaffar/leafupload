class CurrentUserModel {
  final String farmerId;
  final String username;

  CurrentUserModel({
    required this.farmerId,
    required this.username,
  });

  // GET /api/mobile/auth/me returns a flat {farmerId, username} body.
  factory CurrentUserModel.fromJson(Map<String, dynamic> json) {
    return CurrentUserModel(
      farmerId: json['farmerId']?.toString() ?? '',
      username: json['username']?.toString() ?? '',
    );
  }
}
