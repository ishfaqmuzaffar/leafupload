import 'package:firebase_core/firebase_core.dart' show FirebaseOptions;
import 'package:flutter/foundation.dart' show TargetPlatform, defaultTargetPlatform, kIsWeb;

// Hand-written (not `flutterfire configure`-generated). Web and Android apps
// are registered in the krishimitra-ai-jkcip Firebase project; the Android
// values below come from android/app/google-services.json. Add an iOS block
// (with its own GoogleService-Info.plist) once that platform is packaged.
class DefaultFirebaseOptions {
  static FirebaseOptions get currentPlatform {
    if (kIsWeb) {
      return web;
    }
    if (defaultTargetPlatform == TargetPlatform.android) {
      return android;
    }
    throw UnsupportedError(
      'DefaultFirebaseOptions have not been configured for this platform yet - '
      'only Web and Android are set up. Push notifications are unavailable here.',
    );
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
}

// Web push (VAPID) key from Project settings -> Cloud Messaging -> Web
// configuration - needed to fetch an FCM token on the web platform.
const String kFcmVapidKey =
    'BJVMrH0RJVxq1vB8UYRN0vCOkJ5ESQmC7dh8wxiSNYIwEryui_bPqr9nAObO3Er_6qvLXGFfk1pR5uJ8jLkUPP4';
