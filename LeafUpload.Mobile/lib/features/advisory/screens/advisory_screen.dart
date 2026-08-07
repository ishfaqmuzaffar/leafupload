import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_radius.dart';
import 'package:krishimitra_mobile/core/constants/app_spacing.dart';
import 'package:krishimitra_mobile/core/widgets/dashboard_tile.dart';
import 'package:krishimitra_mobile/core/widgets/empty_state_card.dart';
import 'package:krishimitra_mobile/core/widgets/icon_badge.dart';
import 'package:krishimitra_mobile/features/advisory/models/advisory_model.dart';
import 'package:krishimitra_mobile/features/advisory/services/advisory_service.dart';
import 'package:krishimitra_mobile/features/advisory/weather_code_display.dart';
import 'package:krishimitra_mobile/features/farms/models/farm_model.dart';
import 'package:krishimitra_mobile/features/farms/services/farms_service.dart';

class AdvisoryScreen extends StatefulWidget {
  final String username;

  const AdvisoryScreen({super.key, required this.username});

  @override
  State<AdvisoryScreen> createState() => _AdvisoryScreenState();
}

class _FarmAdvisory {
  final FarmModel farm;
  final AdvisoryModel? advisory;
  final String? error;

  _FarmAdvisory({required this.farm, this.advisory, this.error});

  int get activeAlertCount =>
      (advisory?.alerts ?? []).where((a) => a.severity != AdvisorySeverity.info).length;
}

class _AdvisoryScreenState extends State<AdvisoryScreen> {
  final FarmsService _farmsService = FarmsService();
  final AdvisoryService _advisoryService = AdvisoryService();

  bool _isLoading = true;
  String? _errorMessage;
  List<_FarmAdvisory> _farmAdvisories = [];

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      final farms = await _farmsService.getFarms();
      final results = await Future.wait(farms.map((farm) async {
        try {
          final advisory = await _advisoryService.getAdvisoryForFarm(farm.id);
          return _FarmAdvisory(farm: farm, advisory: advisory);
        } catch (_) {
          return _FarmAdvisory(farm: farm, error: 'Advisory unavailable right now.');
        }
      }));

      if (!mounted) return;
      setState(() {
        _farmAdvisories = results;
        _isLoading = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _errorMessage = 'Failed to load your farms. Pull down to try again.';
        _isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final totalAlerts = _farmAdvisories.fold<int>(0, (sum, f) => sum + f.activeAlertCount);

    return Scaffold(
      body: RefreshIndicator(
        onRefresh: _loadData,
        child: CustomScrollView(
          slivers: [
            SliverToBoxAdapter(child: _buildHeader()),
            if (_isLoading)
              const SliverFillRemaining(
                child: Center(child: CircularProgressIndicator(color: AppColors.primary)),
              )
            else if (_errorMessage != null)
              SliverToBoxAdapter(
                child: Padding(
                  padding: const EdgeInsets.all(AppSpacing.lg),
                  child: EmptyStateCard(icon: Icons.error_outline_rounded, title: 'Something went wrong', subtitle: _errorMessage!),
                ),
              )
            else if (_farmAdvisories.isEmpty)
              const SliverToBoxAdapter(
                child: Padding(
                  padding: EdgeInsets.all(AppSpacing.lg),
                  child: EmptyStateCard(
                    icon: Icons.eco_outlined,
                    title: 'No farms yet',
                    subtitle: 'Add your first farm from the My Farms tab to start getting weather-driven crop advisories.',
                  ),
                ),
              )
            else ...[
              SliverPadding(
                padding: const EdgeInsets.fromLTRB(AppSpacing.lg, AppSpacing.lg, AppSpacing.lg, 0),
                sliver: SliverToBoxAdapter(child: _buildStatsRow(totalAlerts)),
              ),
              SliverPadding(
                padding: const EdgeInsets.fromLTRB(AppSpacing.lg, AppSpacing.lg, AppSpacing.lg, 90),
                sliver: SliverList(
                  delegate: SliverChildBuilderDelegate(
                    (context, index) => Padding(
                      padding: const EdgeInsets.only(bottom: AppSpacing.lg),
                      child: _FarmAdvisoryCard(entry: _farmAdvisories[index]),
                    ),
                    childCount: _farmAdvisories.length,
                  ),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildStatsRow(int totalAlerts) {
    return Row(
      children: [
        Expanded(
          child: SizedBox(
            height: 92,
            child: DashboardTile.stat(
              value: '${_farmAdvisories.length}',
              label: 'My Farms',
              backgroundColor: AppColors.primaryDark,
            ),
          ),
        ),
        const SizedBox(width: AppSpacing.md),
        Expanded(
          child: SizedBox(
            height: 92,
            child: DashboardTile.stat(
              value: '$totalAlerts',
              label: totalAlerts == 0 ? 'All clear' : 'Active alerts',
              backgroundColor: totalAlerts == 0 ? AppColors.success : AppColors.ctaOrangeStart,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildHeader() {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.fromLTRB(AppSpacing.lg, AppSpacing.xxl, AppSpacing.lg, AppSpacing.xl),
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [AppColors.headerGradientStart, AppColors.headerGradientEnd],
        ),
      ),
      child: SafeArea(
        bottom: false,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('🌿 KrishiMitra AI', style: TextStyle(color: Colors.white, fontWeight: FontWeight.w700, fontSize: 15)),
            const SizedBox(height: 10),
            Text(
              'Crop Advisories',
              style: Theme.of(context).textTheme.headlineMedium?.copyWith(color: Colors.white, fontWeight: FontWeight.w800),
            ),
            const SizedBox(height: 4),
            Text(
              'Weather-driven guidance for your registered farm${_farmAdvisories.length == 1 ? '' : 's'}.',
              style: const TextStyle(color: Colors.white70, fontSize: 14),
            ),
          ],
        ),
      ),
    );
  }
}

class _FarmAdvisoryCard extends StatelessWidget {
  final _FarmAdvisory entry;

  const _FarmAdvisoryCard({required this.entry});

  void _showAlertDetails(BuildContext context, AdvisoryAlertModel alert) {
    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.transparent,
      isScrollControlled: true,
      builder: (context) => _AlertDetailsSheet(alert: alert),
    );
  }

  @override
  Widget build(BuildContext context) {
    final farm = entry.farm;
    final alerts = entry.advisory?.alerts ?? [];
    final activeAlerts = entry.activeAlertCount;

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppRadius.xxl),
        border: Border.all(color: AppColors.border),
      ),
      clipBehavior: Clip.antiAlias,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(AppSpacing.lg),
            decoration: const BoxDecoration(
              gradient: LinearGradient(colors: [AppColors.headerGradientStart, AppColors.primaryDark]),
            ),
            child: Row(
              children: [
                IconBadge(
                  color: Colors.white24,
                  child: Text(farm.cropIcon, style: const TextStyle(fontSize: 22)),
                ),
                const SizedBox(width: AppSpacing.md),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(farm.cropNameLocalized,
                          style: const TextStyle(color: Colors.white, fontWeight: FontWeight.w700, fontSize: 16)),
                      const SizedBox(height: 2),
                      Text('📍 ${farm.resolvedLocationName ?? farm.placeName}',
                          style: const TextStyle(color: Colors.white70, fontSize: 13)),
                    ],
                  ),
                ),
                if (entry.advisory != null)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                    decoration: BoxDecoration(color: Colors.white24, borderRadius: BorderRadius.circular(999)),
                    child: Text(
                      activeAlerts == 0 ? '✅ All clear' : '⚠️ $activeAlerts alert${activeAlerts == 1 ? '' : 's'}',
                      style: const TextStyle(color: Colors.white, fontSize: 12, fontWeight: FontWeight.w700),
                    ),
                  ),
              ],
            ),
          ),
          if (entry.error != null)
            Padding(
              padding: const EdgeInsets.all(AppSpacing.lg),
              child: Text(entry.error!, style: const TextStyle(color: AppColors.textSecondary)),
            )
          else ...[
            if (entry.advisory?.forecast != null) _WeatherStrip(forecast: entry.advisory!.forecast!),
            Padding(
              padding: const EdgeInsets.fromLTRB(AppSpacing.md, 0, AppSpacing.md, AppSpacing.md),
              child: GridView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: alerts.length,
                gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: 2,
                  mainAxisSpacing: 10,
                  crossAxisSpacing: 10,
                  childAspectRatio: 0.92,
                ),
                itemBuilder: (context, index) => _AlertGridTile(
                  alert: alerts[index],
                  onTap: () => _showAlertDetails(context, alerts[index]),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

(Color bg, Color border, Color text) _severityColors(AdvisorySeverity severity) {
  switch (severity) {
    case AdvisorySeverity.critical:
      return (AppColors.sevCriticalBg, AppColors.sevCriticalBorder, AppColors.sevCriticalText);
    case AdvisorySeverity.warning:
      return (AppColors.sevWarningBg, AppColors.sevWarningBorder, AppColors.sevWarningText);
    case AdvisorySeverity.caution:
      return (AppColors.sevCautionBg, AppColors.sevCautionBorder, AppColors.sevCautionText);
    case AdvisorySeverity.info:
      return (AppColors.sevInfoBg, AppColors.sevInfoBorder, AppColors.sevInfoText);
  }
}

class _WeatherStrip extends StatelessWidget {
  final WeatherForecastModel forecast;

  const _WeatherStrip({required this.forecast});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 104,
      child: ListView.separated(
        padding: const EdgeInsets.symmetric(horizontal: AppSpacing.md, vertical: 8),
        scrollDirection: Axis.horizontal,
        itemCount: forecast.dayCount,
        separatorBuilder: (_, __) => const SizedBox(width: 8),
        itemBuilder: (context, i) {
          final date = DateTime.tryParse(forecast.dates[i]);
          final code = i < forecast.weatherCode.length ? forecast.weatherCode[i] : null;
          return Container(
            width: 66,
            padding: const EdgeInsets.symmetric(vertical: 8),
            decoration: BoxDecoration(
              color: AppColors.background,
              borderRadius: BorderRadius.circular(AppRadius.md),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(date != null ? DateFormat('EEE').format(date) : '', style: const TextStyle(fontSize: 11, fontWeight: FontWeight.w700)),
                const SizedBox(height: 4),
                Text(WeatherCodeDisplay.iconFor(code), style: const TextStyle(fontSize: 18)),
                const SizedBox(height: 4),
                Text(
                  '${forecast.tempMinC[i].round()}°/${forecast.tempMaxC[i].round()}°',
                  style: const TextStyle(fontSize: 11, color: AppColors.textSecondary),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}

class _AlertGridTile extends StatelessWidget {
  final AdvisoryAlertModel alert;
  final VoidCallback onTap;

  const _AlertGridTile({required this.alert, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final (bg, border, text) = _severityColors(alert.severity);

    return DashboardTile(
      icon: IconBadge(
        color: border.withValues(alpha: 0.15),
        child: Text(alert.icon, style: const TextStyle(fontSize: 20)),
      ),
      title: alert.title,
      subtitle: alert.message,
      badgeText: alert.timing,
      badgeColor: border,
      backgroundColor: bg,
      borderColor: border.withValues(alpha: 0.35),
      titleColor: text,
      subtitleColor: AppColors.textSecondary,
      onTap: onTap,
    );
  }
}

class _AlertDetailsSheet extends StatelessWidget {
  final AdvisoryAlertModel alert;

  const _AlertDetailsSheet({required this.alert});

  @override
  Widget build(BuildContext context) {
    final (bg, border, text) = _severityColors(alert.severity);

    return Container(
      padding: EdgeInsets.fromLTRB(AppSpacing.lg, AppSpacing.md, AppSpacing.lg, AppSpacing.lg + MediaQuery.of(context).padding.bottom),
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(AppRadius.xxl)),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Center(
            child: Container(
              width: 40,
              height: 4,
              margin: const EdgeInsets.only(bottom: AppSpacing.lg),
              decoration: BoxDecoration(color: AppColors.border, borderRadius: BorderRadius.circular(999)),
            ),
          ),
          Row(
            children: [
              IconBadge(color: bg, child: Text(alert.icon, style: const TextStyle(fontSize: 22))),
              const SizedBox(width: AppSpacing.md),
              Expanded(
                child: Text(alert.title, style: TextStyle(color: text, fontWeight: FontWeight.w800, fontSize: 17)),
              ),
              if (alert.timing != null)
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                  decoration: BoxDecoration(color: border.withValues(alpha: 0.15), borderRadius: BorderRadius.circular(999)),
                  child: Text(alert.timing!, style: TextStyle(color: text, fontSize: 12, fontWeight: FontWeight.w600)),
                ),
            ],
          ),
          const SizedBox(height: AppSpacing.md),
          Text(alert.message, style: const TextStyle(color: AppColors.textPrimary, fontSize: 14.5, height: 1.5)),
          if (alert.actions.isNotEmpty) ...[
            const SizedBox(height: AppSpacing.md),
            ...alert.actions.map((action) => Padding(
                  padding: const EdgeInsets.only(bottom: 6),
                  child: Text('•  $action', style: const TextStyle(fontSize: 13.5, color: AppColors.textSecondary, height: 1.4)),
                )),
          ],
        ],
      ),
    );
  }
}
