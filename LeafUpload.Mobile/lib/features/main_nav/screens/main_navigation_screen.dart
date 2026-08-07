import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_radius.dart';
import 'package:krishimitra_mobile/features/advisory/screens/advisory_screen.dart';
import 'package:krishimitra_mobile/features/auth/models/current_user_model.dart';
import 'package:krishimitra_mobile/features/diagnosis/screens/diagnosis_screen.dart';
import 'package:krishimitra_mobile/features/farms/screens/farms_screen.dart';
import 'package:krishimitra_mobile/features/profile/screens/profile_screen.dart';

class MainNavigationScreen extends StatefulWidget {
  final CurrentUserModel currentUser;

  const MainNavigationScreen({
    super.key,
    required this.currentUser,
  });

  @override
  State<MainNavigationScreen> createState() => _MainNavigationScreenState();
}

class _MainNavigationScreenState extends State<MainNavigationScreen> {
  int _currentIndex = 0;

  late final List<Widget> _screens = [
    AdvisoryScreen(username: widget.currentUser.username),
    const FarmsScreen(),
    const DiagnosisScreen(),
    ProfileScreen(currentUser: widget.currentUser),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: _screens[_currentIndex],
      bottomNavigationBar: _BottomNavBar(
        currentIndex: _currentIndex,
        onSelect: (index) => setState(() => _currentIndex = index),
      ),
    );
  }
}

// Diagnosis (the camera/AI action) is rendered as a raised circular button
// overlapping the bar, mirroring the reference design's raised center nav
// button - the other three destinations stay as flat icon+label items.
class _BottomNavBar extends StatelessWidget {
  final int currentIndex;
  final ValueChanged<int> onSelect;

  const _BottomNavBar({required this.currentIndex, required this.onSelect});

  static const _diagnosisIndex = 2;

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      top: false,
      child: SizedBox(
        height: 84,
        child: Stack(
          clipBehavior: Clip.none,
          // The bar below has 4 equal columns (Advisory, My Farms, [this
          // button's reserved gap], Profile) - that gap is centered at 5/8
          // of the width, not the Stack's own center, so the raised button
          // must align there (Alignment.x = (5/8 - 1/2) * 2 = 0.25) rather
          // than at Alignment.topCenter or it drifts left into "My Farms".
          alignment: const Alignment(0.25, -1),
          children: [
            Positioned(
              left: 0,
              right: 0,
              bottom: 0,
              child: Container(
                height: 68,
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.vertical(top: Radius.circular(AppRadius.xl)),
                  boxShadow: [
                    BoxShadow(color: Colors.black.withValues(alpha: 0.06), blurRadius: 16, offset: const Offset(0, -4)),
                  ],
                ),
                child: Row(
                  children: [
                    Expanded(
                      child: _NavItem(
                        icon: Icons.notifications_outlined,
                        selectedIcon: Icons.notifications_rounded,
                        label: 'Advisory',
                        selected: currentIndex == 0,
                        onTap: () => onSelect(0),
                      ),
                    ),
                    Expanded(
                      child: _NavItem(
                        icon: Icons.landscape_outlined,
                        selectedIcon: Icons.landscape_rounded,
                        label: 'My Farms',
                        selected: currentIndex == 1,
                        onTap: () => onSelect(1),
                      ),
                    ),
                    const Expanded(child: SizedBox.shrink()),
                    Expanded(
                      child: _NavItem(
                        icon: Icons.person_outline_rounded,
                        selectedIcon: Icons.person_rounded,
                        label: 'Profile',
                        selected: currentIndex == 3,
                        onTap: () => onSelect(3),
                      ),
                    ),
                  ],
                ),
              ),
            ),
            GestureDetector(
              onTap: () => onSelect(_diagnosisIndex),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Container(
                    height: 60,
                    width: 60,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      gradient: const LinearGradient(
                        colors: [AppColors.ctaOrangeStart, AppColors.ctaOrangeEnd],
                      ),
                      boxShadow: [
                        BoxShadow(
                          color: AppColors.ctaOrangeStart.withValues(alpha: 0.4),
                          blurRadius: 14,
                          offset: const Offset(0, 6),
                        ),
                      ],
                    ),
                    child: Icon(
                      currentIndex == _diagnosisIndex ? Icons.biotech_rounded : Icons.biotech_outlined,
                      color: Colors.white,
                      size: 28,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'Diagnosis',
                    style: TextStyle(
                      fontSize: 11,
                      fontWeight: FontWeight.w600,
                      color: currentIndex == _diagnosisIndex ? AppColors.ctaOrangeStart : AppColors.textMuted,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _NavItem extends StatelessWidget {
  final IconData icon;
  final IconData selectedIcon;
  final String label;
  final bool selected;
  final VoidCallback onTap;

  const _NavItem({
    required this.icon,
    required this.selectedIcon,
    required this.label,
    required this.selected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final color = selected ? AppColors.primary : AppColors.textMuted;
    return InkWell(
      onTap: onTap,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(selected ? selectedIcon : icon, color: color, size: 24),
          const SizedBox(height: 4),
          Text(label, style: TextStyle(fontSize: 11, fontWeight: FontWeight.w600, color: color)),
        ],
      ),
    );
  }
}
