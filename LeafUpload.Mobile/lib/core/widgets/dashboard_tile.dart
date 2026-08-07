import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_radius.dart';

/// A rounded card used across the dashboard-style screens (Advisory alerts
/// grid, My Farms grid): an icon + title + subtitle tile with an optional
/// corner badge. Use [DashboardTile.stat] for a solid-color stat highlight
/// tile (big number + label) instead.
class DashboardTile extends StatelessWidget {
  final Widget? icon;
  final String title;
  final String? subtitle;
  final String? badgeText;
  final Color? badgeColor;
  final Color backgroundColor;
  final Color borderColor;
  final Color titleColor;
  final Color subtitleColor;
  final bool isStat;
  final VoidCallback? onTap;

  DashboardTile({
    super.key,
    this.icon,
    required this.title,
    this.subtitle,
    this.badgeText,
    this.badgeColor,
    this.backgroundColor = Colors.white,
    this.borderColor = AppColors.border,
    this.titleColor = AppColors.textPrimary,
    this.subtitleColor = AppColors.textSecondary,
    this.onTap,
  }) : isStat = false;

  DashboardTile.stat({
    super.key,
    required String value,
    required String label,
    required this.backgroundColor,
    this.titleColor = Colors.white,
    Color? subtitleColor,
  })  : icon = null,
        title = value,
        subtitle = label,
        badgeText = null,
        badgeColor = null,
        borderColor = Colors.transparent,
        subtitleColor = subtitleColor ?? Colors.white70,
        isStat = true,
        onTap = null;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: backgroundColor,
      borderRadius: BorderRadius.circular(AppRadius.lg),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppRadius.lg),
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(AppRadius.lg),
            // A transparent border still reserves its width in the layout
            // box, which was clipping the stat-tile variant's text by
            // exactly the border width - so skip it entirely when unused.
            border: borderColor == Colors.transparent ? null : Border.all(color: borderColor),
          ),
          padding: const EdgeInsets.all(14),
          child: isStat ? _buildStat() : _buildDefault(),
        ),
      ),
    );
  }

  Widget _buildStat() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisAlignment: MainAxisAlignment.end,
      children: [
        Text(subtitle ?? '', style: TextStyle(color: subtitleColor, fontSize: 12, fontWeight: FontWeight.w600)),
        const SizedBox(height: 4),
        Text(title, style: TextStyle(color: titleColor, fontSize: 30, fontWeight: FontWeight.w800)),
      ],
    );
  }

  Widget _buildDefault() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (icon != null) icon! else const SizedBox.shrink(),
            if (badgeText != null)
              Flexible(
                child: Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: (badgeColor ?? AppColors.primary).withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(999),
                  ),
                  child: Text(
                    badgeText!,
                    style: TextStyle(color: badgeColor ?? AppColors.primary, fontSize: 10.5, fontWeight: FontWeight.w700),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
              ),
          ],
        ),
        const SizedBox(height: 10),
        Text(
          title,
          style: TextStyle(color: titleColor, fontWeight: FontWeight.w700, fontSize: 14.5),
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
        ),
        if (subtitle != null) ...[
          const SizedBox(height: 2),
          Text(
            subtitle!,
            style: TextStyle(color: subtitleColor, fontSize: 12.5),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
        ],
      ],
    );
  }
}
