// Mirrors LeafUpload.Core/Models/WeatherCodeDisplay.cs exactly so the mobile
// weather strip matches the web app's icons for the same WMO codes.
class WeatherCodeDisplay {
  static String iconFor(int? code) {
    switch (code) {
      case 0:
        return '☀️';
      case 1:
      case 2:
      case 3:
        return '⛅';
      case 45:
      case 48:
        return '🌫️';
      case 51:
      case 53:
      case 55:
      case 56:
      case 57:
        return '🌦️';
      case 61:
      case 63:
      case 65:
      case 66:
      case 67:
        return '🌧️';
      case 71:
      case 73:
      case 75:
      case 77:
        return '🌨️';
      case 80:
      case 81:
      case 82:
        return '🌦️';
      case 85:
      case 86:
        return '🌨️';
      case 95:
        return '⛈️';
      case 96:
      case 99:
        return '🧊';
      default:
        return '🌤️';
    }
  }
}
