import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/theme/app_theme.dart';
import 'package:krishimitra_mobile/features/splash/screens/splash_screen.dart';

class KrishiMitraApp extends StatelessWidget {
  const KrishiMitraApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'KrishiMitra AI',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.lightTheme,
      home: const SplashScreen(),
    );
  }
}