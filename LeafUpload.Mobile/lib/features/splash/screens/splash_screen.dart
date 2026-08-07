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

class _SplashScreenState extends State<SplashScreen> {
  final AuthService _authService = AuthService();

  @override
  void initState() {
    super.initState();
    _bootstrap();
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
    return const Scaffold(
      backgroundColor: AppColors.background,
      body: Center(
        child: Padding(
          padding: EdgeInsets.all(AppSpacing.xl),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              _SplashLogo(),
              SizedBox(height: AppSpacing.lg),
              Text(
                'KrishiMitra AI',
                style: TextStyle(
                  fontSize: 30,
                  fontWeight: FontWeight.w800,
                  color: AppColors.textPrimary,
                ),
              ),
              SizedBox(height: 10),
              Text(
                'Weather-smart crop advisories and instant leaf diagnosis',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 15,
                  color: AppColors.textSecondary,
                ),
              ),
              SizedBox(height: AppSpacing.xl),
              CircularProgressIndicator(
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
    return CircleAvatar(
      radius: 42,
      backgroundColor: AppColors.primaryLight,
      child: Icon(
        Icons.eco_rounded,
        size: 42,
        color: AppColors.primary,
      ),
    );
  }
}