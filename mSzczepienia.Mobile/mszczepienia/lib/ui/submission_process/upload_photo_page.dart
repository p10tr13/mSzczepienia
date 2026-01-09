import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/core/utils/image_picker_helper.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';

class UploadPhotoPage extends StatefulWidget {
  final String imagePaths;

  const UploadPhotoPage({super.key, required this.imagePaths});

  @override
  State<UploadPhotoPage> createState() => _UploadPhotoPageState();
}

class _UploadPhotoPageState extends State<UploadPhotoPage> {
  final ImagePickerHelper imagePickerHelper = ImagePickerHelper();
  late List<String> decodedImagePaths;

  void showPhotoSourceDialog() {
    ImagePickerHelper.showPhotoSourceDialog(
      context: context,
      onCameraPicked: pickSingleImage,
      onGalleryPicked: pickMultipleImages,
    );
  }

  Future<void> pickSingleImage() async {
    final path = await imagePickerHelper.pickImage(ImageSource.camera);
    if (path != null && mounted && !decodedImagePaths.contains(path)) {
      setState(() {
        decodedImagePaths.add(path);
      });
    }
  }

  Future<void> pickMultipleImages() async {
    final paths = await imagePickerHelper.pickMultipleImages();
    if (paths.isNotEmpty &&
        mounted &&
        !decodedImagePaths.any((path) => paths.contains(path))) {
      setState(() {
        decodedImagePaths.addAll(paths);
      });
    }
  }

  @override
  void initState() {
    super.initState();
    decodedImagePaths = ImagePickerHelper.decodeImagePaths(widget.imagePaths);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;

    return Layout(
      child: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        child: Padding(
          padding: const EdgeInsets.only(
            left: 24.0,
            right: 24.0,
            top: 8.0,
            bottom: 8.0,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    l10n.vaccinationBooklet,
                    style: TextStyle(fontSize: 32, fontWeight: FontWeight.w800),
                  ),
                  const SizedBox(height: 35),
                  if (decodedImagePaths.isNotEmpty)
                    ...decodedImagePaths.asMap().entries.map((entry) {
                      final index = entry.key;
                      final imagePath = entry.value;
                      return Padding(
                        padding: const EdgeInsets.only(bottom: 16.0),
                        child: Center(
                          child: Stack(
                            children: [
                              Container(
                                width: MediaQuery.of(context).size.width - 48,
                                decoration: BoxDecoration(
                                  border: Border.all(
                                    color: AppColors.defaultBlue,
                                    width: 2.0,
                                  ),
                                  borderRadius: BorderRadius.circular(16.0),
                                ),
                                child: ClipRRect(
                                  borderRadius: BorderRadius.circular(16.0),
                                  child: Image.file(
                                    File(imagePath),
                                    width:
                                        MediaQuery.of(context).size.width - 48,
                                    fit: BoxFit.contain,
                                  ),
                                ),
                              ),
                              Positioned(
                                top: 8,
                                right: 8,
                                child: GestureDetector(
                                  onTap: () {
                                    setState(() {
                                      decodedImagePaths.removeAt(index);
                                    });
                                  },
                                  child: Icon(
                                    Icons.close,
                                    color: AppColors.darkGrey,
                                    size: 24.0,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      );
                    }),
                  const SizedBox(height: 16),
                ],
              ),
              Column(
                children: [
                  SizedBox(
                    width: double.infinity,
                    height: 56,
                    child: OutlinedButton(
                      onPressed: () {
                        showPhotoSourceDialog();
                      },
                      style: OutlinedButton.styleFrom(
                        side: BorderSide(color: borderColor, width: 2.0),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(30),
                        ),
                      ),
                      child: Icon(Icons.add_a_photo_rounded, size: 20.0),
                    ),
                  ),
                  const SizedBox(height: 16),
                  SizedBox(
                    width: double.infinity,
                    height: 56,
                    child: BlocBuilder<OcrSubmissionCubit, OcrSubmissionState>(
                      builder: (context, state) {
                        return ElevatedButton(
                          onPressed: () {
                            if (decodedImagePaths.isNotEmpty &&
                                state is! OcrSubmissionProcessingLoading) {
                              final imageFiles = decodedImagePaths
                                  .map((path) => File(path))
                                  .toList();
                              context
                                  .read<OcrSubmissionCubit>()
                                  .processVaccinationBooklet(imageFiles);
                              context.go('/submission-status/analyze');
                            }
                          },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppColors.defaultBlue,
                            elevation: 0,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(30),
                            ),
                          ),
                          child: Text(
                            l10n.analize,
                            style: const TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.w600,
                              color: Colors.white,
                            ),
                          ),
                        );
                      },
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
