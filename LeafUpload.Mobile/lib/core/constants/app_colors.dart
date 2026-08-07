import 'package:flutter/material.dart';

// Mirrors the web app's brand palette (LeafUpload.Web/Views/Shared/_Layout.cshtml
// CSS variables --leaf-bright/--leaf-accent/--header-green-*/--cta-orange-*) so the
// mobile app reads as the same product, not a reskin of a different brand.
class AppColors {
  static const Color primary = Color(0xFF22C55E); // --leaf-bright
  static const Color primaryDark = Color(0xFF14532D); // --header-green-2
  static const Color primaryLight = Color(0xFFE8F5E9);

  static const Color accentTeal = Color(0xFF34D9C4); // --leaf-accent
  static const Color secondary = Color(0xFF34D9C4);

  static const Color headerGradientStart = Color(0xFF0F5132); // --header-green-1
  static const Color headerGradientEnd = Color(0xFF14532D); // --header-green-2

  static const Color ctaOrangeStart = Color(0xFFF97316); // --cta-orange-1
  static const Color ctaOrangeEnd = Color(0xFFFB923C); // --cta-orange-2

  static const Color background = Color(0xFFF6F8F5);
  static const Color surface = Colors.white;
  static const Color border = Color(0xFFE2E8E0);

  static const Color textPrimary = Color(0xFF1F2937);
  static const Color textSecondary = Color(0xFF6B7280);
  static const Color textMuted = Color(0xFF9CA3AF);

  static const Color success = Color(0xFF16A34A);
  static const Color warning = Color(0xFFF59E0B);
  static const Color danger = Color(0xFFDC2626);
  static const Color info = Color(0xFF2563EB);

  // Advisory alert severity colors - mirror the web app's .sev-critical/.sev-warning/
  // .sev-caution/.sev-info classes exactly so both clients agree on what "critical"
  // looks like.
  static const Color sevCriticalBg = Color(0xFFFEF2F2);
  static const Color sevCriticalBorder = Color(0xFFEF4444);
  static const Color sevCriticalText = Color(0xFF991B1B);

  static const Color sevWarningBg = Color(0xFFFFF7ED);
  static const Color sevWarningBorder = Color(0xFFF59E0B);
  static const Color sevWarningText = Color(0xFF92400E);

  static const Color sevCautionBg = Color(0xFFFEFCE8);
  static const Color sevCautionBorder = Color(0xFFCA8A04);
  static const Color sevCautionText = Color(0xFF854D0E);

  static const Color sevInfoBg = Color(0xFFEFF6FF);
  static const Color sevInfoBorder = Color(0xFF3B82F6);
  static const Color sevInfoText = Color(0xFF1E40AF);
}
