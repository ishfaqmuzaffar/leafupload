import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_spacing.dart';
import 'package:krishimitra_mobile/core/widgets/dashboard_tile.dart';
import 'package:krishimitra_mobile/core/widgets/empty_state_card.dart';
import 'package:krishimitra_mobile/core/widgets/icon_badge.dart';
import 'package:krishimitra_mobile/features/crops/models/crop_model.dart';
import 'package:krishimitra_mobile/features/crops/services/crops_service.dart';
import 'package:krishimitra_mobile/features/farms/models/farm_model.dart';
import 'package:krishimitra_mobile/features/farms/screens/create_farm_screen.dart';
import 'package:krishimitra_mobile/features/farms/services/farms_service.dart';

class FarmsScreen extends StatefulWidget {
  const FarmsScreen({super.key});

  @override
  State<FarmsScreen> createState() => _FarmsScreenState();
}

class _FarmsScreenState extends State<FarmsScreen> {
  final FarmsService _farmsService = FarmsService();
  final CropsService _cropsService = CropsService();

  bool _isLoading = true;
  String? _errorMessage;

  List<CropModel> _crops = [];
  List<FarmModel> _farms = [];

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
      final results = await Future.wait([
        _cropsService.getCrops(),
        _farmsService.getFarms(),
      ]);

      if (!mounted) return;

      setState(() {
        _crops = results[0] as List<CropModel>;
        _farms = results[1] as List<FarmModel>;
        _isLoading = false;
      });
    } catch (_) {
      if (!mounted) return;

      setState(() {
        _errorMessage = 'Failed to load farms. Please try again.';
        _isLoading = false;
      });
    }
  }

  Future<void> _openCreateFarm() async {
    if (_crops.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('No crops available. Please try again later.'), backgroundColor: AppColors.danger),
      );
      return;
    }

    final created = await Navigator.push<bool>(
      context,
      MaterialPageRoute(builder: (_) => CreateFarmScreen(crops: _crops)),
    );

    if (created == true) {
      await _loadData();
    }
  }

  Widget _buildFarmCard(FarmModel farm) {
    return DashboardTile(
      icon: IconBadge(
        color: AppColors.primaryLight,
        child: Text(farm.cropIcon, style: const TextStyle(fontSize: 22)),
      ),
      title: farm.resolvedLocationName ?? farm.placeName,
      subtitle: farm.cropNameLocalized,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('My Farms'),
        actions: [
          IconButton(onPressed: _loadData, icon: const Icon(Icons.refresh_rounded), tooltip: 'Refresh'),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _openCreateFarm,
        icon: const Icon(Icons.add),
        label: const Text('Add Farm'),
      ),
      body: RefreshIndicator(
        onRefresh: _loadData,
        child: _isLoading
            ? const Center(child: CircularProgressIndicator(color: AppColors.primary))
            : _errorMessage != null
                ? ListView(
                    padding: const EdgeInsets.all(AppSpacing.lg),
                    children: [
                      EmptyStateCard(icon: Icons.error_outline_rounded, title: 'Something went wrong', subtitle: _errorMessage!),
                    ],
                  )
                : _farms.isEmpty
                    ? ListView(
                        padding: const EdgeInsets.all(AppSpacing.lg),
                        children: const [
                          EmptyStateCard(
                            icon: Icons.landscape_outlined,
                            title: 'No farms added yet',
                            subtitle: 'Add your first farm to start getting weather-driven crop advisories.',
                          ),
                        ],
                      )
                    : GridView.builder(
                        padding: const EdgeInsets.fromLTRB(AppSpacing.lg, AppSpacing.lg, AppSpacing.lg, 90),
                        itemCount: _farms.length,
                        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                          crossAxisCount: 2,
                          mainAxisSpacing: AppSpacing.md,
                          crossAxisSpacing: AppSpacing.md,
                          childAspectRatio: 0.95,
                        ),
                        itemBuilder: (context, index) => _buildFarmCard(_farms[index]),
                      ),
      ),
    );
  }
}
