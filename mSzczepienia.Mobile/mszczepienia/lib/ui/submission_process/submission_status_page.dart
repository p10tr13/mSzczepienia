import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/core/utils/image_picker_helper.dart';
import 'package:mszczepienia/logic/citizen/citizen_cubit.dart';
import 'package:mszczepienia/logic/citizen/citizen_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';

class SubmissionStatusPage extends StatefulWidget {
  const SubmissionStatusPage({super.key});

  @override
  State<SubmissionStatusPage> createState() => _SubmissionStatusPageState();
}

class _SubmissionStatusPageState extends State<SubmissionStatusPage> {
  late String submissionStatus;
  late Color statusColor;
  late IconData statusIcon;
  final ImagePickerHelper imagePickerHelper = ImagePickerHelper();

  void _showPhotoSourceDialog() {
    ImagePickerHelper.showPhotoSourceDialog(
      context: context,
      onCameraPicked: _pickSingleImage,
      onGalleryPicked: _pickMultipleImages,
    );
  }

  Future<void> _pickSingleImage() async {
    final path = await imagePickerHelper.pickImage(ImageSource.camera);
    if (path != null && mounted) {
      final encodedPaths = ImagePickerHelper.encodeImagePath(path);
      context.push('/upload-photo?imagePaths=$encodedPaths');
    }
  }

  Future<void> _pickMultipleImages() async {
    final paths = await imagePickerHelper.pickMultipleImages();
    if (paths.isNotEmpty && mounted) {
      final encodedPaths = ImagePickerHelper.encodeImagePaths(paths);
      context.push('/upload-photo?imagePaths=$encodedPaths');
    }
  }

  void getSubmissionStatus(String status, AppLocalizations l10n) {
    switch (status) {
      case 'NotUploaded':
        submissionStatus = l10n.statusNotUploaded;
        statusColor = AppColors.darkGrey;
        statusIcon = Icons.hourglass_empty;
        break;
      case 'Pending':
        submissionStatus = l10n.statusPending;
        statusColor = AppColors.defaultBlue;
        statusIcon = Icons.hourglass_top;
        break;
      case 'Approved':
        submissionStatus = l10n.statusApproved;
        statusColor = Colors.green;
        statusIcon = Icons.check_circle;
        break;
      case 'Rejected':
        submissionStatus = l10n.statusRejected;
        statusColor = Colors.red;
        statusIcon = Icons.cancel;
        break;
      default:
        submissionStatus = l10n.statusNotUploaded;
        statusColor = AppColors.darkGrey;
        statusIcon = Icons.hourglass_empty;
    }
  }

  @override
  void initState() {
    super.initState();
    context.read<CitizenCubit>().getCitizenStatus();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;

    return Layout(
      child: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.only(
            left: 24.0,
            right: 24.0,
            top: 8.0,
            bottom: 8.0,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                l10n.vaccinationBooklet,
                style: TextStyle(fontSize: 32, fontWeight: FontWeight.w800),
              ),
              const SizedBox(height: 35),

              InkWell(
                onTap: () {
                  _showPhotoSourceDialog();
                },
                borderRadius: BorderRadius.circular(16),
                child: Container(
                  padding: const EdgeInsets.all(10),
                  width: double.infinity,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: borderColor, width: 1.1),
                  ),
                  child: Row(
                    children: [
                      Container(
                        padding: EdgeInsets.only(left: 12.0, right: 2.0),
                        child: ElevatedButton(
                          onPressed: () {
                            _showPhotoSourceDialog();
                          },
                          style: ElevatedButton.styleFrom(
                            shape: CircleBorder(),
                            padding: EdgeInsets.all(7.0),
                            backgroundColor: AppColors.lightGrey,
                            foregroundColor: AppColors.lightGrey,
                          ),
                          child: Icon(
                            Icons.camera_alt_rounded,
                            color: Colors.black,
                            size: 20.0,
                          ),
                        ),
                      ),
                      const SizedBox(width: 15),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              l10n.uploadNewPhoto,
                              style: TextStyle(
                                fontSize: 15,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            const SizedBox(height: 10),
                            Text(
                              l10n.updateYourVaccinationHistory,
                              style: TextStyle(fontSize: 14),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 22),
              Container(
                padding: const EdgeInsets.all(10),
                width: double.infinity,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: borderColor, width: 1.1),
                ),
                child: BlocBuilder<CitizenCubit, CitizenState>(
                  builder: (context, state) {
                    if (state is CitizenLoading) {
                      return Center(
                        child: SizedBox(
                          width: 32,
                          height: 32,
                          child: CircularProgressIndicator(
                            color: AppColors.defaultBlue,
                            strokeWidth: 2.5,
                          ),
                        ),
                      );
                    } else if (state is CitizenLoaded) {
                      getSubmissionStatus(state.status.submissionStatus, l10n);
                    } else {
                      getSubmissionStatus('NotUploaded', l10n);
                    }
                    return Row(
                      children: [
                        Container(
                          padding: EdgeInsets.only(left: 12.0, right: 2.0),
                          child: ElevatedButton(
                            onPressed: () {},
                            style: ElevatedButton.styleFrom(
                              shape: CircleBorder(),
                              padding: EdgeInsets.all(7.0),
                              backgroundColor: AppColors.lightGrey,
                              foregroundColor: AppColors.lightGrey,
                            ),
                            child: Icon(
                              statusIcon,
                              color: Colors.black,
                              size: 20.0,
                            ),
                          ),
                        ),
                        const SizedBox(width: 15),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                l10n.submissionStatus,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                submissionStatus,
                                style: TextStyle(
                                  fontSize: 14,
                                  color: statusColor,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    );
                  },
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
