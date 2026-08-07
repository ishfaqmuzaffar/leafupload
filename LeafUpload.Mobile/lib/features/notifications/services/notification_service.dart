import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:krishimitra_mobile/features/notifications/services/devices_service.dart';
import 'package:krishimitra_mobile/firebase_options.dart';

// Requests notification permission, fetches an FCM token, and registers it
// with the backend so weather alerts can be pushed to this device/browser.
// Wired up for Web, Android, and iOS (see firebase_options.dart) - safe to
// call on any platform since every step is guarded/caught, so a missing
// Firebase config (e.g. on Windows/macOS desktop) just skips push
// registration instead of crashing login.
class NotificationService {
  final DevicesService _devicesService = DevicesService();

  bool get _supported =>
      kIsWeb || defaultTargetPlatform == TargetPlatform.android || defaultTargetPlatform == TargetPlatform.iOS;

  Future<void> requestPermissionAndRegister() async {
    if (!_supported) return;

    try {
      final settings = await FirebaseMessaging.instance.requestPermission();
      if (settings.authorizationStatus == AuthorizationStatus.denied) {
        return;
      }

      final token = kIsWeb
          ? await FirebaseMessaging.instance.getToken(vapidKey: kFcmVapidKey)
          : await FirebaseMessaging.instance.getToken();
      if (token != null && token.isNotEmpty) {
        final platform = kIsWeb ? 'web' : (defaultTargetPlatform == TargetPlatform.iOS ? 'ios' : 'android');
        await _devicesService.registerToken(token, platform);
      }
    } catch (e) {
      debugPrint('Push notification setup skipped: $e');
    }
  }
}
