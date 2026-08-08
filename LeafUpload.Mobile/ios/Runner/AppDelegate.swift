import Flutter
import UIKit

@main
@objc class AppDelegate: FlutterAppDelegate, FlutterImplicitEngineDelegate {
  override func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {
    // With the implicit-engine architecture, GeneratedPluginRegistrant.register
    // (which is what firebase_messaging normally uses to trigger this) only runs
    // in didInitializeImplicitFlutterEngine, below - after this method returns.
    // Call it directly here instead so iOS starts the APNs handshake as early as
    // possible, rather than never completing it for this launch.
    application.registerForRemoteNotifications()
    return super.application(application, didFinishLaunchingWithOptions: launchOptions)
  }

  func didInitializeImplicitFlutterEngine(_ engineBridge: FlutterImplicitEngineBridge) {
    GeneratedPluginRegistrant.register(with: engineBridge.pluginRegistry)
  }
}
