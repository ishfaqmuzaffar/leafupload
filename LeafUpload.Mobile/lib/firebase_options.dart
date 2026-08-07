import 'package:firebase_core/firebase_core.dart' show FirebaseOptions;
import 'package:flutter/foundation.dart' show TargetPlatform, defaultTargetPlatform, kIsWeb;

// Hand-written (not `flutterfire configure`-generated). Web, Android, and iOS
// apps are registered in the krishimitra-ai-jkcip Firebase project; the
// Android values come from android/app/google-services.json, the iOS values
// from ios/Runner/GoogleService-Info.plist.
class DefaultFirebaseOptions {
  static FirebaseOptions get currentPlatform {
    if (kIsWeb) {
      return web;
    }
    switch (defaultTargetPlatform) {
      case TargetPlatform.android:
        return android;
      case TargetPlatform.iOS:
        return ios;
      default:
        throw UnsupportedError(
          'DefaultFirebaseOptions have not been configured for this platform yet - '
          'only Web, Android, and iOS are set up. Push notifications are unavailable here.',
        );
    }
  }

  static const FirebaseOptions web = FirebaseOptions(
    apiKey: 'AIzaSyBxvSAOXs2E1BZW1Z-KObDUd4U2xy-NM8o',
    authDomain: 'krishimitra-ai-jkcip.firebaseapp.com',
    projectId: 'krishimitra-ai-jkcip',
    storageBucket: 'krishimitra-ai-jkcip.firebasestorage.app',
    messagingSenderId: '1051741103315',
    appId: '1:1051741103315:web:84e8f7296456da762b6026',
    measurementId: 'G-N9HX7LHN7K',
  );

  static const FirebaseOptions android = FirebaseOptions(
    apiKey: 'AIzaSyB92QCruzU5x6D4ZQ-SFne7yBTNXSPTYec',
    projectId: 'krishimitra-ai-jkcip',
    storageBucket: 'krishimitra-ai-jkcip.firebasestorage.app',
    messagingSenderId: '1051741103315',
    appId: '1:1051741103315:android:9bc6f67ec27ba8472b6026',
  );

  static const FirebaseOptions ios = FirebaseOptions(
    apiKey: 'AIzaSyA7P0KiyyEueiZcskH-FhstKL6cGsO30Cg',
    projectId: 'krishimitra-ai-jkcip',
    storageBucket: 'krishimitra-ai-jkcip.firebasestorage.app',
    messagingSenderId: '1051741103315',
    appId: '1:1051741103315:ios:706f4ef2dee66b0f2b6026',
    iosBundleId: 'com.jkcip.krishimitra',
  );
}

// Web push (VAPID) key from Project settings -> Cloud Messaging -> Web
// configuration - needed to fetch an FCM token on the web platform.
const String kFcmVapidKey =
    'BJVMrH0RJVxq1vB8UYRN0vCOkJ5ESQmC7dh8wxiSNYIwEryui_bPqr9nAObO3Er_6qvLXGFfk1pR5uJ8jLkUPP4';
