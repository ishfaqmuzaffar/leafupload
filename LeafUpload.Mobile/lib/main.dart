import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'app.dart';
import 'firebase_options.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Only Web, Android, and iOS are configured in firebase_options.dart so far
  // - skip elsewhere (e.g. Windows desktop) rather than crash startup.
  // Failures here shouldn't block the app loading either, since push
  // notifications are a nice-to-have.
  if (kIsWeb || defaultTargetPlatform == TargetPlatform.android || defaultTargetPlatform == TargetPlatform.iOS) {
    try {
      await Firebase.initializeApp(options: DefaultFirebaseOptions.currentPlatform);

      // iOS silently drops notifications while the app is in the foreground
      // unless told otherwise (Android shows them either way) - without this,
      // a farmer with the app open would never see a weather alert land.
      if (!kIsWeb && defaultTargetPlatform == TargetPlatform.iOS) {
        await FirebaseMessaging.instance.setForegroundNotificationPresentationOptions(
          alert: true,
          badge: true,
          sound: true,
        );
      }
    } catch (e) {
      debugPrint('Firebase init skipped: $e');
    }
  }

  runApp(const KrishiMitraApp());
}
