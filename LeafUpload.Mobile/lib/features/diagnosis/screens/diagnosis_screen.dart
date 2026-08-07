import 'dart:typed_data';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:krishimitra_mobile/core/constants/app_colors.dart';
import 'package:krishimitra_mobile/core/constants/app_radius.dart';
import 'package:krishimitra_mobile/core/constants/app_spacing.dart';
import 'package:krishimitra_mobile/core/widgets/icon_badge.dart';
import 'package:krishimitra_mobile/core/widgets/primary_button.dart';
import 'package:krishimitra_mobile/features/diagnosis/models/diagnosis_result_model.dart';
import 'package:krishimitra_mobile/features/diagnosis/services/diagnosis_service.dart';

class DiagnosisScreen extends StatefulWidget {
  const DiagnosisScreen({super.key});

  @override
  State<DiagnosisScreen> createState() => _DiagnosisScreenState();
}

class _DiagnosisScreenState extends State<DiagnosisScreen> {
  final ImagePicker _imagePicker = ImagePicker();
  final DiagnosisService _diagnosisService = DiagnosisService();

  XFile? _selectedImage;
  Uint8List? _selectedImageBytes;
  bool _isDiagnosing = false;
  String? _errorMessage;
  DiagnosisResultModel? _result;

  Future<void> _pickImage(ImageSource source) async {
    final picked = await _imagePicker.pickImage(source: source, imageQuality: 85);
    if (picked == null) return;

    final bytes = await picked.readAsBytes();

    setState(() {
      _selectedImage = picked;
      _selectedImageBytes = bytes;
      _result = null;
      _errorMessage = null;
    });
  }

  Future<void> _diagnose() async {
    if (_selectedImage == null) return;

    setState(() {
      _isDiagnosing = true;
      _errorMessage = null;
      _result = null;
    });

    try {
      final result = await _diagnosisService.diagnose(_selectedImage!);
      if (!mounted) return;
      setState(() {
        _result = result;
        _isDiagnosing = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _errorMessage = 'Could not diagnose this photo. Please try again.';
        _isDiagnosing = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Leaf Diagnosis')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(AppSpacing.lg),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Diagnose a leaf',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
            ),
            const SizedBox(height: 4),
            Text(
              'Take or upload a clear photo of a single leaf and our AI model will identify the disease and suggest treatment.',
              style: Theme.of(context).textTheme.bodyMedium,
            ),
            const SizedBox(height: AppSpacing.lg),
            _buildImagePreview(),
            const SizedBox(height: AppSpacing.md),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () => _pickImage(ImageSource.camera),
                    icon: const Icon(Icons.photo_camera_outlined),
                    label: const Text('Camera'),
                  ),
                ),
                const SizedBox(width: AppSpacing.md),
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () => _pickImage(ImageSource.gallery),
                    icon: const Icon(Icons.photo_library_outlined),
                    label: const Text('Gallery'),
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppSpacing.lg),
            PrimaryButton(
              text: '🔍 Diagnose Leaf',
              onPressed: _selectedImage == null ? null : _diagnose,
              isLoading: _isDiagnosing,
            ),
            if (_errorMessage != null) ...[
              const SizedBox(height: AppSpacing.md),
              Text(_errorMessage!, style: const TextStyle(color: AppColors.danger)),
            ],
            if (_result != null) ...[
              const SizedBox(height: AppSpacing.lg),
              _DiagnosisResultCard(result: _result!),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildImagePreview() {
    return Container(
      height: 220,
      width: double.infinity,
      decoration: BoxDecoration(
        color: AppColors.primaryLight,
        borderRadius: BorderRadius.circular(AppRadius.xxl),
        border: Border.all(color: AppColors.border),
      ),
      clipBehavior: Clip.antiAlias,
      child: _selectedImageBytes != null
          ? Image.memory(_selectedImageBytes!, fit: BoxFit.cover)
          : const Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.image_search_rounded, size: 48, color: AppColors.primary),
                  SizedBox(height: 8),
                  Text('No photo selected yet', style: TextStyle(color: AppColors.textSecondary)),
                ],
              ),
            ),
    );
  }
}

class _DiagnosisResultCard extends StatelessWidget {
  final DiagnosisResultModel result;

  const _DiagnosisResultCard({required this.result});

  @override
  Widget build(BuildContext context) {
    final accent = result.isHealthy ? AppColors.success : AppColors.danger;
    final confidencePct = (result.confidence * 100).toStringAsFixed(0);

    return Container(
      padding: const EdgeInsets.all(AppSpacing.lg),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppRadius.xxl),
        border: Border.all(color: accent.withValues(alpha: 0.35)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              IconBadge(
                color: accent.withValues(alpha: 0.12),
                child: Icon(result.isHealthy ? Icons.check_circle_rounded : Icons.warning_amber_rounded, color: accent),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  result.displayName,
                  style: TextStyle(color: accent, fontWeight: FontWeight.w800, fontSize: 18),
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          Text('Confidence: $confidencePct%', style: const TextStyle(color: AppColors.textSecondary, fontSize: 13)),
          if (result.symptoms.isNotEmpty) ...[
            const SizedBox(height: AppSpacing.md),
            const Text('Symptoms', style: TextStyle(fontWeight: FontWeight.w700)),
            const SizedBox(height: 6),
            ...result.symptoms.map((s) => Padding(
                  padding: const EdgeInsets.only(bottom: 4),
                  child: Text('•  $s', style: const TextStyle(fontSize: 13.5, color: AppColors.textPrimary)),
                )),
          ],
          if (result.treatment.isNotEmpty) ...[
            const SizedBox(height: AppSpacing.md),
            const Text('Recommended treatment', style: TextStyle(fontWeight: FontWeight.w700)),
            const SizedBox(height: 6),
            Text(result.treatment, style: const TextStyle(fontSize: 13.5, color: AppColors.textPrimary, height: 1.4)),
          ],
        ],
      ),
    );
  }
}
