import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_radius.dart';
import 'package:krishimitra_mobile/core/constants/app_spacing.dart';
import 'package:krishimitra_mobile/core/widgets/icon_badge.dart';
import 'package:krishimitra_mobile/features/auth/models/current_user_model.dart';
import 'package:krishimitra_mobile/features/auth/screens/login_screen.dart';
import 'package:krishimitra_mobile/features/auth/services/auth_service.dart';
import 'package:krishimitra_mobile/features/notifications/services/notification_service.dart';

class ProfileScreen extends StatefulWidget {
  final CurrentUserModel currentUser;

  const ProfileScreen({
    super.key,
    required this.currentUser,
  });

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  // Push notifications are wired up for Web, Android, and iOS (see
  // firebase_options.dart) - hide the toggle elsewhere (e.g. Windows desktop)
  // rather than show a control that can't do anything.
  bool get _pushSupported =>
      kIsWeb || defaultTargetPlatform == TargetPlatform.android || defaultTargetPlatform == TargetPlatform.iOS;

  bool? _notificationsEnabled;

  @override
  void initState() {
    super.initState();
    if (_pushSupported) _refreshNotificationStatus();
  }

  Future<void> _refreshNotificationStatus() async {
    try {
      final settings = await FirebaseMessaging.instance.getNotificationSettings();
      if (!mounted) return;
      setState(() {
        _notificationsEnabled = settings.authorizationStatus == AuthorizationStatus.authorized;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() => _notificationsEnabled = false);
    }
  }

  Future<void> _onNotificationsToggled(bool value) async {
    if (value) {
      await NotificationService().requestPermissionAndRegister();
    } else if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('To turn off notifications, disable them in your device Settings.')),
      );
    }
    await _refreshNotificationStatus();
  }

  Future<void> _logout(BuildContext context) async {
    await AuthService().logout();

    if (!context.mounted) return;

    Navigator.pushAndRemoveUntil(
      context,
      MaterialPageRoute(
        builder: (_) => const LoginScreen(),
      ),
      (route) => false,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Profile'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(AppSpacing.lg),
        child: Column(
          children: [
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(AppSpacing.xl),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(AppRadius.xxl),
                border: Border.all(color: AppColors.border),
              ),
              child: Column(
                children: [
                  CircleAvatar(
                    radius: 34,
                    backgroundColor: AppColors.primaryLight,
                    child: Text(
                      widget.currentUser.username.isNotEmpty ? widget.currentUser.username[0].toUpperCase() : 'K',
                      style: const TextStyle(
                        fontSize: 26,
                        fontWeight: FontWeight.w700,
                        color: AppColors.primary,
                      ),
                    ),
                  ),
                  const SizedBox(height: AppSpacing.md),
                  Text(
                    widget.currentUser.username,
                    style: Theme.of(context).textTheme.titleLarge?.copyWith(
                          fontWeight: FontWeight.w700,
                        ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    'Farmer',
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                          color: AppColors.primary,
                          fontWeight: FontWeight.w600,
                        ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: AppSpacing.lg),
            _SettingsGroup(
              children: [
                if (_pushSupported)
                  _SettingsRow(
                    icon: Icons.notifications_active_outlined,
                    iconColor: AppColors.primary,
                    label: 'Push Notifications',
                    trailing: Switch(
                      value: _notificationsEnabled ?? false,
                      activeTrackColor: AppColors.primary,
                      onChanged: _onNotificationsToggled,
                    ),
                  ),
                _SettingsRow(
                  icon: Icons.logout_rounded,
                  iconColor: AppColors.danger,
                  label: 'Logout',
                  onTap: () => _logout(context),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _SettingsGroup extends StatelessWidget {
  final List<Widget> children;

  const _SettingsGroup({required this.children});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppRadius.xxl),
        border: Border.all(color: AppColors.border),
      ),
      clipBehavior: Clip.antiAlias,
      child: Column(
        children: [
          for (int i = 0; i < children.length; i++) ...[
            if (i > 0) const Divider(height: 1, color: AppColors.border),
            children[i],
          ],
        ],
      ),
    );
  }
}

class _SettingsRow extends StatelessWidget {
  final IconData icon;
  final Color iconColor;
  final String label;
  final Widget? trailing;
  final VoidCallback? onTap;

  const _SettingsRow({
    required this.icon,
    required this.iconColor,
    required this.label,
    this.trailing,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.lg, vertical: AppSpacing.sm),
        child: Row(
          children: [
            IconBadge(color: iconColor.withValues(alpha: 0.12), size: 38, child: Icon(icon, color: iconColor, size: 20)),
            const SizedBox(width: AppSpacing.md),
            Expanded(
              child: Text(label, style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 15, color: AppColors.textPrimary)),
            ),
            trailing ?? const Icon(Icons.chevron_right_rounded, color: AppColors.textMuted),
          ],
        ),
      ),
    );
  }
}
