import 'package:flutter/material.dart';

/// A circular colored container for an icon/emoji - the "colorful icon"
/// treatment used throughout the dashboard-style screens (Advisory alerts,
/// farm cards, diagnosis results) instead of a bare Icon widget.
class IconBadge extends StatelessWidget {
  final Widget child;
  final Color color;
  final double size;

  const IconBadge({
    super.key,
    required this.child,
    required this.color,
    this.size = 44,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: size,
      width: size,
      decoration: BoxDecoration(color: color, shape: BoxShape.circle),
      alignment: Alignment.center,
      child: child,
    );
  }
}
