import 'dart:async';

import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_spacing.dart';
import 'package:krishimitra_mobile/features/auth/services/auth_service.dart';
import 'package:krishimitra_mobile/features/landing/screens/landing_screen.dart';
import 'package:krishimitra_mobile/features/main_nav/screens/main_navigation_screen.dart';
import 'package:krishimitra_mobile/features/notifications/services/notification_service.dart';

class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> with SingleTickerProviderStateMixin {
  final AuthService _authService = AuthService();
  late final AnimationController _pulseController;
  late final Animation<double> _pulseAnimation;

  @override
  void initState() {
    super.initState();
    _pulseController = AnimationController(vsync: this, duration: const Duration(milliseconds: 1100))
      ..repeat(reverse: true);
    _pulseAnimation = Tween<double>(begin: 0.88, end: 1.08).animate(
      CurvedAnimation(parent: _pulseController, curve: Curves.easeInOut),
    );
    _bootstrap();
  }

  @override
  void dispose() {
    _pulseController.dispose();
    super.dispose();
  }

  Future<void> _bootstrap() async {
    await Future.delayed(const Duration(milliseconds: 900));

    final user = await _authService.getCurrentUser();

    if (!mounted) return;

    if (user != null) {
      unawaited(NotificationService().requestPermissionAndRegister());
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (_) => MainNavigationScreen(currentUser: user),
        ),
      );
      return;
    }

    Navigator.pushReplacement(
      context,
      MaterialPageRoute(
        builder: (_) => const LandingScreen(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(AppSpacing.xl),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              ScaleTransition(scale: _pulseAnimation, child: const _SplashLogo()),
              const SizedBox(height: AppSpacing.lg),
              const Text(
                'KrishiMitra AI',
                style: TextStyle(
                  fontSize: 30,
                  fontWeight: FontWeight.w800,
                  color: AppColors.textPrimary,
                ),
              ),
              const SizedBox(height: 10),
              const Text(
                'Weather-smart crop advisories and instant leaf diagnosis',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 15,
                  color: AppColors.textSecondary,
                ),
              ),
              const SizedBox(height: AppSpacing.xl),
              const CircularProgressIndicator(
                color: AppColors.primary,
              )
            ],
          ),
        ),
      ),
    );
  }
}

class _SplashLogo extends StatelessWidget {
  const _SplashLogo();

  @override
  Widget build(BuildContext context) {
    return Image.asset(
      'assets/icon/app_icon.png',
      height: 120,
      width: 120,
    );
  }
}