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

      // On iOS, FCM can't hand out a token until Apple's own APNs token has
      // been delivered to the app, which happens asynchronously right after
      // requestPermission() - calling getToken() immediately is a race that
      // fails with "apns-token-not-set". Poll for it first; this has been
      // observed to take well over 5s on some networks, so allow up to 30s.
      if (!kIsWeb && defaultTargetPlatform == TargetPlatform.iOS) {
        String? apnsToken = await FirebaseMessaging.instance.getAPNSToken();
        var attempts = 0;
        while (apnsToken == null && attempts < 60) {
          await Future.delayed(const Duration(milliseconds: 500));
          apnsToken = await FirebaseMessaging.instance.getAPNSToken();
          attempts++;
          if (attempts % 4 == 0) {
            debugPrint('Still waiting for APNs token... (${attempts * 500}ms)');
          }
        }
        debugPrint(apnsToken == null
            ? 'Gave up waiting for APNs token after ${attempts * 500}ms'
            : 'Got APNs token after ${attempts * 500}ms');
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
