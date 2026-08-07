import 'package:geolocator/geolocator.dart';

class LocationResult {
  final double latitude;
  final double longitude;

  LocationResult(this.latitude, this.longitude);
}

// Wraps the geolocator permission dance so screens just call one method and
// get either a position or a human-readable reason it failed.
class LocationService {
  Future<LocationResult> getCurrentLocation() async {
    if (!await Geolocator.isLocationServiceEnabled()) {
      throw Exception('Location services are turned off. Please enable them and try again.');
    }

    var permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
      if (permission == LocationPermission.denied) {
        throw Exception('Location permission was denied.');
      }
    }

    if (permission == LocationPermission.deniedForever) {
      throw Exception('Location permission is permanently denied. Enable it in your browser/device settings.');
    }

    final position = await Geolocator.getCurrentPosition(
      locationSettings: const LocationSettings(accuracy: LocationAccuracy.high),
    );
    return LocationResult(position.latitude, position.longitude);
  }
}
