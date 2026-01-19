import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';

class ImagePickerHelper {
  final ImagePicker _picker = ImagePicker();

  static const int maxWidth = 1920;
  static const int maxHeight = 1080;
  static const int imageQuality = 85;

  /// Shows a bottom sheet dialog to select photo source
  static void showPhotoSourceDialog({
    required BuildContext context,
    required Future<void> Function() onCameraPicked,
    required Future<void> Function() onGalleryPicked,
  }) {
    final l10n = AppLocalizations.of(context)!;

    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16.0)),
      ),
      builder: (BuildContext context) {
        return SafeArea(
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 20.0),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  l10n.selectPhotoSource,
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 20),
                ListTile(
                  leading: Container(
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: AppColors.lightGrey,
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: const Icon(
                      Icons.camera_alt_rounded,
                      color: Colors.black,
                    ),
                  ),
                  title: Text(l10n.takePhoto),
                  onTap: () async {
                    Navigator.pop(context);
                    await onCameraPicked();
                  },
                ),
                ListTile(
                  leading: Container(
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: AppColors.lightGrey,
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: const Icon(
                      Icons.photo_library_rounded,
                      color: Colors.black,
                    ),
                  ),
                  title: Text(l10n.chooseFromGallery),
                  onTap: () async {
                    Navigator.pop(context);
                    await onGalleryPicked();
                  },
                ),
                const SizedBox(height: 10),
                TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: Text(
                    l10n.cancel,
                    style: const TextStyle(fontSize: 16),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  /// Picks a single image from the specified source (camera or gallery)
  /// Returns the file path or null if cancelled
  Future<String?> pickImage(ImageSource source) async {
    try {
      final XFile? pickedFile = await _picker.pickImage(
        source: source,
        maxWidth: maxWidth.toDouble(),
        maxHeight: maxHeight.toDouble(),
        imageQuality: imageQuality,
      );

      return pickedFile?.path;
    } catch (e) {
      debugPrint('Error picking image: $e');
      return null;
    }
  }

  /// Picks multiple images from gallery
  /// Returns list of file paths or empty list if cancelled
  Future<List<String>> pickMultipleImages() async {
    try {
      final List<XFile> pickedFiles = await _picker.pickMultiImage(
        maxWidth: maxWidth.toDouble(),
        maxHeight: maxHeight.toDouble(),
        imageQuality: imageQuality,
      );

      return pickedFiles.map((f) => f.path).toList();
    } catch (e) {
      debugPrint('Error picking images: $e');
      return [];
    }
  }

  /// Encodes image paths for URL navigation
  /// Joins multiple paths with '|' separator
  static String encodeImagePaths(List<String> paths) {
    return Uri.encodeComponent(paths.join('|'));
  }

  /// Encodes a single image path for URL navigation
  static String encodeImagePath(String path) {
    return Uri.encodeComponent(path);
  }

  /// Decodes image paths from URL parameter
  static List<String> decodeImagePaths(String encodedPaths) {
    if (encodedPaths.isEmpty) return [];
    final decoded = Uri.decodeComponent(encodedPaths);
    return decoded.split('|');
  }
}
