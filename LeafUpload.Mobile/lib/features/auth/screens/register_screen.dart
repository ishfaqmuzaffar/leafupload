import 'package:flutter/material.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_spacing.dart';
import 'package:krishimitra_mobile/core/location/location_service.dart';
import 'package:krishimitra_mobile/core/storage/token_storage.dart';
import 'package:krishimitra_mobile/core/widgets/app_logo_header.dart';
import 'package:krishimitra_mobile/core/widgets/app_text_field.dart';
import 'package:krishimitra_mobile/core/widgets/primary_button.dart';
import 'package:krishimitra_mobile/features/auth/services/auth_service.dart';
import 'package:krishimitra_mobile/features/crops/models/crop_model.dart';
import 'package:krishimitra_mobile/features/crops/services/crops_service.dart';
import 'package:krishimitra_mobile/features/crops/services/geo_service.dart';
import 'package:krishimitra_mobile/features/main_nav/screens/main_navigation_screen.dart';
import 'package:krishimitra_mobile/features/notifications/services/notification_service.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();

  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  final _placeNameController = TextEditingController();

  final AuthService _authService = AuthService();
  final TokenStorage _tokenStorage = TokenStorage();
  final CropsService _cropsService = CropsService();
  final LocationService _locationService = LocationService();
  final GeoService _geoService = GeoService();

  List<CropModel> _crops = [];
  String? _selectedCrop;
  bool _isLoading = false;
  bool _isLoadingCrops = true;
  bool _isLocating = false;
  double? _latitude;
  double? _longitude;

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
  void initState() {
    super.initState();
    _loadCrops();
  }

  Future<void> _loadCrops() async {
    try {
      final crops = await _cropsService.getCrops();
      if (!mounted) return;
      setState(() {
        _crops = crops;
        _isLoadingCrops = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() => _isLoadingCrops = false);
    }
  }

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    _placeNameController.dispose();
    super.dispose();
  }

  Future<void> _register() async {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedCrop == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select a crop'), backgroundColor: AppColors.danger),
      );
      return;
    }

    setState(() => _isLoading = true);

    final result = await _authService.register(
      username: _usernameController.text.trim(),
      password: _passwordController.text.trim(),
      placeName: _placeNameController.text.trim(),
      cropType: _selectedCrop!,
      latitude: _latitude,
      longitude: _longitude,
    );

    if (!mounted) return;

    if (result.success && result.token != null) {
      await _tokenStorage.saveToken(result.token!);
      final user = await _authService.getCurrentUser();

      if (!mounted) return;

      setState(() => _isLoading = false);

      if (user != null) {
        NotificationService().requestPermissionAndRegister();
        Navigator.pushAndRemoveUntil(
          context,
          MaterialPageRoute(
            builder: (_) => MainNavigationScreen(currentUser: user),
          ),
          (route) => false,
        );
        return;
      }
    }

    setState(() => _isLoading = false);

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(result.message),
        backgroundColor: AppColors.danger,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Create account'),
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(AppSpacing.xl),
          child: Center(
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 430),
              child: Form(
                key: _formKey,
                child: Column(
                  children: [
                    const AppLogoHeader(
                      title: 'Join KrishiMitra AI',
                      subtitle: 'Create your farmer account to get weather advisories for your farm.',
                    ),
                    const SizedBox(height: AppSpacing.xl),
                    Card(
                      child: Padding(
                        padding: const EdgeInsets.all(AppSpacing.lg),
                        child: Column(
                          children: [
                            AppTextField(
                              controller: _usernameController,
                              label: 'Username',
                              hint: 'Choose a username',
                              validator: (value) {
                                if (value == null || value.trim().length < 3) {
                                  return 'Username must be at least 3 characters';
                                }
                                return null;
                              },
                            ),
                            const SizedBox(height: AppSpacing.md),
                            AppTextField(
                              controller: _passwordController,
                              label: 'Password',
                              hint: 'Create a password',
                              obscureText: true,
                              validator: (value) {
                                if (value == null || value.trim().isEmpty) {
                                  return 'Password is required';
                                }
                                if (value.trim().length < 6) {
                                  return 'Password must be at least 6 characters';
                                }
                                return null;
                              },
                            ),
                            const SizedBox(height: AppSpacing.md),
                            AppTextField(
                              controller: _confirmPasswordController,
                              label: 'Confirm password',
                              hint: 'Re-enter your password',
                              obscureText: true,
                              validator: (value) {
                                if (value != _passwordController.text) {
                                  return 'Passwords do not match';
                                }
                                return null;
                              },
                            ),
                            const SizedBox(height: AppSpacing.md),
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
                                    ? const SizedBox(
                                        height: 16,
                                        width: 16,
                                        child: CircularProgressIndicator(strokeWidth: 2),
                                      )
                                    : const Icon(Icons.my_location_rounded, size: 18),
                                label: Text(_isLocating ? 'Locating...' : '📍 Use my current location'),
                              ),
                            ),
                            const SizedBox(height: AppSpacing.sm),
                            _isLoadingCrops
                                ? const Padding(
                                    padding: EdgeInsets.symmetric(vertical: AppSpacing.md),
                                    child: CircularProgressIndicator(),
                                  )
                                : DropdownButtonFormField<String>(
                                    initialValue: _selectedCrop,
                                    decoration: const InputDecoration(labelText: 'Crop type'),
                                    items: _crops
                                        .map((crop) => DropdownMenuItem(
                                              value: crop.name,
                                              child: Text('${crop.icon} ${crop.localizedName}'),
                                            ))
                                        .toList(),
                                    onChanged: (value) => setState(() => _selectedCrop = value),
                                  ),
                            const SizedBox(height: AppSpacing.lg),
                            PrimaryButton(
                              text: 'Create account',
                              onPressed: _register,
                              isLoading: _isLoading,
                            ),
                          ],
                        ),
                      ),
                    )
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
