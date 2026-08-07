import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_spacing.dart';
import 'package:krishimitra_mobile/core/location/location_service.dart';
import 'package:krishimitra_mobile/core/widgets/app_text_field.dart';
import 'package:krishimitra_mobile/core/widgets/primary_button.dart';
import 'package:krishimitra_mobile/features/crops/models/crop_model.dart';
import 'package:krishimitra_mobile/features/crops/services/geo_service.dart';
import 'package:krishimitra_mobile/features/farms/services/farms_service.dart';

class CreateFarmScreen extends StatefulWidget {
  final List<CropModel> crops;

  const CreateFarmScreen({super.key, required this.crops});

  @override
  State<CreateFarmScreen> createState() => _CreateFarmScreenState();
}

class _CreateFarmScreenState extends State<CreateFarmScreen> {
  final _formKey = GlobalKey<FormState>();
  final _placeNameController = TextEditingController();

  final FarmsService _farmsService = FarmsService();
  final LocationService _locationService = LocationService();
  final GeoService _geoService = GeoService();

  CropModel? _selectedCrop;
  bool _isSaving = false;
  bool _isLocating = false;
  double? _latitude;
  double? _longitude;

  @override
  void initState() {
    super.initState();
    if (widget.crops.isNotEmpty) {
      _selectedCrop = widget.crops.first;
    }
  }

  Future<void> _useCurrentLocation() async {
    setState(() => _isLocating = true);
    try {
      final location = await _locationService.getCurrentLocation();
      final resolvedName = await _geoService.reverseGeocode(location.latitude, location.longitude);

      if (!mounted) return;
      setState(() {
        _latitude = location.latitude;
        _longitude = location.longitude;
        if (resolvedName != null && resolvedName.isNotEmpty) {
          _placeNameController.text = resolvedName;
        }
        _isLocating = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _isLocating = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(e.toString().replaceFirst('Exception: ', '')), backgroundColor: AppColors.danger),
      );
    }
  }

  @override
  void dispose() {
    _placeNameController.dispose();
    super.dispose();
  }

  Future<void> _saveFarm() async {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedCrop == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select a crop'), backgroundColor: AppColors.danger),
      );
      return;
    }

    setState(() => _isSaving = true);

    final error = await _farmsService.createFarm(
      placeName: _placeNameController.text.trim(),
      cropType: _selectedCrop!.name,
      latitude: _latitude,
      longitude: _longitude,
    );

    if (!mounted) return;

    setState(() => _isSaving = false);

    if (error == null) {
      Navigator.pop(context, true);
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(error), backgroundColor: AppColors.danger),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Add Farm')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(AppSpacing.lg),
        child: Form(
          key: _formKey,
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(AppSpacing.lg),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Farm details',
                    style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    'We use the location to fetch weather-driven advisories for this farm.',
                    style: Theme.of(context).textTheme.bodyMedium,
                  ),
                  const SizedBox(height: AppSpacing.lg),
                  AppTextField(
                    controller: _placeNameController,
                    label: 'Farm location (village/town)',
                    hint: 'e.g. Anantnag',
                    validator: (value) {
                      if (value == null || value.trim().isEmpty) {
                        return 'Farm location is required';
                      }
                      return null;
                    },
                  ),
                  const SizedBox(height: 6),
                  Align(
                    alignment: Alignment.centerLeft,
                    child: TextButton.icon(
                      onPressed: _isLocating ? null : _useCurrentLocation,
                      icon: _isLocating
                          ? const SizedBox(height: 16, width: 16, child: CircularProgressIndicator(strokeWidth: 2))
                          : const Icon(Icons.my_location_rounded, size: 18),
                      label: Text(_isLocating ? 'Locating...' : '📍 Use my current location'),
                    ),
                  ),
                  const SizedBox(height: AppSpacing.sm),
                  Text(
                    'Crop',
                    style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w600),
                  ),
                  const SizedBox(height: 8),
                  DropdownButtonFormField<CropModel>(
                    initialValue: _selectedCrop,
                    decoration: const InputDecoration(hintText: 'Select crop'),
                    items: widget.crops
                        .map((crop) => DropdownMenuItem(
                              value: crop,
                              child: Text('${crop.icon} ${crop.localizedName}'),
                            ))
                        .toList(),
                    onChanged: (value) => setState(() => _selectedCrop = value),
                    validator: (value) => value == null ? 'Please select a crop' : null,
                  ),
                  const SizedBox(height: AppSpacing.lg),
                  PrimaryButton(
                    text: 'Save Farm',
                    onPressed: _saveFarm,
                    isLoading: _isSaving,
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
