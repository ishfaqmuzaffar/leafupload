import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_radius.dart';
import 'package:krishimitra_mobile/core/constants/app_spacing.dart';
import 'package:krishimitra_mobile/features/auth/screens/login_screen.dart';
import 'package:krishimitra_mobile/features/auth/screens/register_screen.dart';
import 'package:krishimitra_mobile/features/crops/models/crop_model.dart';
import 'package:krishimitra_mobile/features/crops/services/crops_service.dart';

// Mobile-styled version of the web portal's public landing page
// (Views/Home/Landing.cshtml) - shown to logged-out users before they
// register or sign in.
class LandingScreen extends StatefulWidget {
  const LandingScreen({super.key});

  @override
  State<LandingScreen> createState() => _LandingScreenState();
}

class _LandingScreenState extends State<LandingScreen> {
  final CropsService _cropsService = CropsService();
  List<CropModel> _crops = [];

  @override
  void initState() {
    super.initState();
    _loadCrops();
  }

  Future<void> _loadCrops() async {
    try {
      final crops = await _cropsService.getCrops();
      if (!mounted) return;
      setState(() => _crops = crops);
    } catch (_) {
      // Landing page still works fine without the crop chips if the API's unreachable.
    }
  }

  void _goToRegister() {
    Navigator.push(context, MaterialPageRoute(builder: (_) => const RegisterScreen()));
  }

  void _goToLogin() {
    Navigator.push(context, MaterialPageRoute(builder: (_) => const LoginScreen()));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: AppSpacing.lg, vertical: AppSpacing.xl),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildHero(),
              const SizedBox(height: AppSpacing.xl),
              _buildStatGrid(),
              const SizedBox(height: AppSpacing.xxl),
              const _SectionHeading(title: 'About the JKCIP Programme'),
              const SizedBox(height: AppSpacing.sm),
              _buildAboutCard(),
              const SizedBox(height: AppSpacing.xxl),
              const _FocusCard(
                icon: '🌦️',
                title: 'Climate-Smart, Market-Led Production',
                body: 'Helping farmers adopt climate-resilient practices and diversify into high-value niche and horticultural crops.',
              ),
              const SizedBox(height: AppSpacing.md),
              const _FocusCard(
                icon: '🤝',
                title: 'Agri-Business Ecosystem Development',
                body: 'Strengthening farmer collectives and the value chain that connects fields to processors and markets.',
              ),
              const SizedBox(height: AppSpacing.md),
              const _FocusCard(
                icon: '🌱',
                title: 'Support for Vulnerable Communities',
                body: 'Targeted support for women, youth, and vulnerable households to share in agricultural growth.',
              ),
              const SizedBox(height: AppSpacing.xxl),
              const _SectionHeading(title: '⚙️ How It Works'),
              const SizedBox(height: AppSpacing.md),
              const _StepItem(icon: '📍', title: 'Register & Locate', body: "Create a free account, tell us your crop, and pin your farm's exact location."),
              const SizedBox(height: AppSpacing.md),
              const _StepItem(icon: '🌦️', title: 'We Watch the Weather', body: "Every day we pull a 7-day forecast for your farm's coordinates from Open-Meteo."),
              const SizedBox(height: AppSpacing.md),
              const _StepItem(icon: '🛡️', title: 'Get Actionable Advisories', body: 'Our engine flags hail, frost, heat waves and more, with crop-specific guidance on what to do next.'),
              const SizedBox(height: AppSpacing.xxl),
              const _SectionHeading(title: '🌾 Crops We Support', subtitle: 'The leaf-disease model and crop advisories both cover these 14 crops.'),
              const SizedBox(height: AppSpacing.md),
              _buildCropGrid(),
              const SizedBox(height: AppSpacing.xxl),
              const _SectionHeading(
                title: '⚠️ Weather Risks We Monitor',
                subtitle: "Our advisory engine watches for each of these in your farm's 7-day forecast.",
              ),
              const SizedBox(height: AppSpacing.md),
              const _RiskCard(icon: '🧊', title: 'Hail', body: 'Thunderstorms with hail can shred leaves and bruise or split fruit within minutes, especially damaging near harvest.'),
              const SizedBox(height: AppSpacing.sm),
              const _RiskCard(icon: '🌡️', title: 'Heat Wave', body: 'Sustained high temperatures raise water demand and can scald sun-exposed fruit and stress flowering crops.'),
              const SizedBox(height: AppSpacing.sm),
              const _RiskCard(icon: '🥶', title: 'Frost', body: "Near-freezing overnight lows can kill blossoms and young shoots, wiping out a season's fruit set."),
              const SizedBox(height: AppSpacing.sm),
              const _RiskCard(icon: '💨', title: 'Windstorm', body: 'Strong winds strip fruit, snap limbs, lodge tall crops, and tear row covers and trellising loose.'),
              const SizedBox(height: AppSpacing.sm),
              const _RiskCard(icon: '🌧️', title: 'Heavy Rainfall', body: 'Waterlogging suffocates roots and washes away treatments, while wet canopies invite fungal disease.'),
              const SizedBox(height: AppSpacing.sm),
              const _RiskCard(icon: '💧', title: 'High Humidity', body: 'Prolonged humidity favors fungal and bacterial disease - scab, brown rot, mildew, and blight spread faster.'),
              const SizedBox(height: AppSpacing.xxl),
              _buildCtaBanner(),
              const SizedBox(height: AppSpacing.lg),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildHero() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
          decoration: BoxDecoration(
            gradient: const LinearGradient(colors: [AppColors.primary, AppColors.accentTeal]),
            borderRadius: BorderRadius.circular(999),
          ),
          child: const Text('🌿 KrishiMitra AI', style: TextStyle(color: Colors.white, fontWeight: FontWeight.w800, fontSize: 13)),
        ),
        const SizedBox(height: AppSpacing.md),
        Text(
          'Weather-Smart Farming for Jammu & Kashmir',
          style: Theme.of(context).textTheme.headlineMedium?.copyWith(fontWeight: FontWeight.w800, color: AppColors.primaryDark),
        ),
        const SizedBox(height: AppSpacing.sm),
        Text(
          "Krishi Mitra AI is JKCIP's intelligent crop advisory platform that combines weather intelligence and AI to help farmers make better decisions - farm-specific weather advisories, early risk alerts, and instant AI-powered leaf diagnosis, all in one place.",
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(color: AppColors.textSecondary, height: 1.5),
        ),
        const SizedBox(height: AppSpacing.lg),
        Row(
          children: [
            Expanded(
              child: ElevatedButton(
                onPressed: _goToRegister,
                child: const Text('🌾 Register Your Farm'),
              ),
            ),
          ],
        ),
        const SizedBox(height: AppSpacing.sm),
        Row(
          children: [
            Expanded(
              child: OutlinedButton(
                onPressed: _goToLogin,
                child: const Text('Already have an account? Log in'),
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildStatGrid() {
    const stats = [
      ('912,523', '👤 Farmers Onboarded'),
      ('425,321', '🌱 Farms Registered'),
      ('141,878', '🏭 Units Tracking'),
      ('2,000', '🏢 Kisan Khidmat Ghars Onboarded'),
    ];

    return GridView.count(
      crossAxisCount: 2,
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      mainAxisSpacing: AppSpacing.sm,
      crossAxisSpacing: AppSpacing.sm,
      childAspectRatio: 1.5,
      children: stats
          .map((s) => Container(
                padding: const EdgeInsets.all(AppSpacing.md),
                decoration: BoxDecoration(
                  gradient: const LinearGradient(begin: Alignment.topLeft, end: Alignment.bottomRight, colors: [Colors.white, AppColors.primaryLight]),
                  borderRadius: BorderRadius.circular(AppRadius.lg),
                  border: Border.all(color: AppColors.border),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(s.$1, style: const TextStyle(fontSize: 20, fontWeight: FontWeight.w800, color: AppColors.primaryDark)),
                    const SizedBox(height: 4),
                    Text(s.$2, style: const TextStyle(fontSize: 11.5, color: AppColors.textSecondary, fontWeight: FontWeight.w600)),
                  ],
                ),
              ))
          .toList(),
    );
  }

  Widget _buildAboutCard() {
    return Container(
      padding: const EdgeInsets.all(AppSpacing.lg),
      decoration: BoxDecoration(
        color: AppColors.primaryLight,
        borderRadius: BorderRadius.circular(AppRadius.lg),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'This advisory tool supports the Jammu & Kashmir Competitiveness Improvement of Agriculture and Allied Sectors Project (JKCIP), an IFAD-supported initiative under the Horticulture & Agriculture Development Programme (HADP). JKCIP works with farmers across the Union Territory to improve productivity, climate resilience, and market access through a value-chain approach.',
            style: TextStyle(fontSize: 13.5, color: AppColors.textPrimary, height: 1.5),
          ),
          const SizedBox(height: AppSpacing.md),
          Row(
            children: const [
              Expanded(child: _JkcipStat(value: '47%', label: 'Women-led households reached', sub: '(141,000)')),
              Expanded(child: _JkcipStat(value: '30%', label: 'Youth households reached', sub: '(90,000)')),
              Expanded(child: _JkcipStat(value: '10%', label: 'Vulnerable-community households', sub: '(30,000)')),
            ],
          ),
          const SizedBox(height: AppSpacing.sm),
          const Text(
            'Programme-wide figures published on the official JKCIP portal, shown here for context - not specific to this tool.',
            style: TextStyle(fontSize: 11, color: AppColors.textMuted, fontStyle: FontStyle.italic),
          ),
        ],
      ),
    );
  }

  Widget _buildCropGrid() {
    if (_crops.isEmpty) {
      return const SizedBox(height: 40, child: Center(child: CircularProgressIndicator(strokeWidth: 2)));
    }
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: _crops
          .map((crop) => Container(
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                decoration: BoxDecoration(
                  color: Colors.white,
                  border: Border.all(color: AppColors.border),
                  borderRadius: BorderRadius.circular(999),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(crop.icon, style: const TextStyle(fontSize: 15)),
                    const SizedBox(width: 6),
                    Text(crop.localizedName, style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: AppColors.textPrimary)),
                  ],
                ),
              ))
          .toList(),
    );
  }

  Widget _buildCtaBanner() {
    return Container(
      padding: const EdgeInsets.all(AppSpacing.lg),
      decoration: BoxDecoration(
        gradient: const LinearGradient(colors: [AppColors.headerGradientStart, AppColors.headerGradientEnd]),
        borderRadius: BorderRadius.circular(AppRadius.lg),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text('Ready to protect your crops?', style: TextStyle(color: Colors.white, fontWeight: FontWeight.w800, fontSize: 18)),
          const SizedBox(height: 6),
          const Text(
            'Registration takes less than a minute and advisories start generating right away.',
            style: TextStyle(color: Colors.white70, fontSize: 13),
          ),
          const SizedBox(height: AppSpacing.md),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: _goToRegister,
              child: const Text('🌾 Register Your Farm'),
            ),
          ),
        ],
      ),
    );
  }
}

class _SectionHeading extends StatelessWidget {
  final String title;
  final String? subtitle;

  const _SectionHeading({required this.title, this.subtitle});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(title, style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w800, color: AppColors.primaryDark)),
        if (subtitle != null) ...[
          const SizedBox(height: 4),
          Text(subtitle!, style: const TextStyle(fontSize: 13, color: AppColors.textSecondary)),
        ],
      ],
    );
  }
}

class _JkcipStat extends StatelessWidget {
  final String value;
  final String label;
  final String sub;

  const _JkcipStat({required this.value, required this.label, required this.sub});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(value, style: const TextStyle(fontSize: 20, fontWeight: FontWeight.w800, color: AppColors.ctaOrangeStart)),
        const SizedBox(height: 2),
        Text(label, textAlign: TextAlign.center, style: const TextStyle(fontSize: 10.5, fontWeight: FontWeight.w600, color: AppColors.textSecondary)),
        Text(sub, style: const TextStyle(fontSize: 10, color: AppColors.textMuted)),
      ],
    );
  }
}

class _FocusCard extends StatelessWidget {
  final String icon;
  final String title;
  final String body;

  const _FocusCard({required this.icon, required this.title, required this.body});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(AppSpacing.md),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppRadius.lg),
        border: Border.all(color: AppColors.border),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(icon, style: const TextStyle(fontSize: 26)),
          const SizedBox(width: AppSpacing.md),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title, style: const TextStyle(fontWeight: FontWeight.w700, fontSize: 14.5, color: AppColors.textPrimary)),
                const SizedBox(height: 4),
                Text(body, style: const TextStyle(fontSize: 12.5, color: AppColors.textSecondary, height: 1.4)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _StepItem extends StatelessWidget {
  final String icon;
  final String title;
  final String body;

  const _StepItem({required this.icon, required this.title, required this.body});

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          height: 44,
          width: 44,
          decoration: BoxDecoration(color: AppColors.primaryLight, borderRadius: BorderRadius.circular(14)),
          child: Center(child: Text(icon, style: const TextStyle(fontSize: 20))),
        ),
        const SizedBox(width: AppSpacing.md),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(title, style: const TextStyle(fontWeight: FontWeight.w700, fontSize: 14.5)),
              const SizedBox(height: 2),
              Text(body, style: const TextStyle(fontSize: 12.5, color: AppColors.textSecondary, height: 1.4)),
            ],
          ),
        ),
      ],
    );
  }
}

class _RiskCard extends StatelessWidget {
  final String icon;
  final String title;
  final String body;

  const _RiskCard({required this.icon, required this.title, required this.body});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(AppSpacing.md),
      decoration: BoxDecoration(
        color: AppColors.sevInfoBg,
        borderRadius: BorderRadius.circular(AppRadius.md),
        border: Border.all(color: AppColors.sevInfoBorder.withValues(alpha: 0.25)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(icon, style: const TextStyle(fontSize: 20)),
          const SizedBox(width: AppSpacing.sm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title, style: const TextStyle(fontWeight: FontWeight.w700, fontSize: 13.5, color: AppColors.sevInfoText)),
                const SizedBox(height: 2),
                Text(body, style: const TextStyle(fontSize: 12, color: AppColors.textSecondary, height: 1.4)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
