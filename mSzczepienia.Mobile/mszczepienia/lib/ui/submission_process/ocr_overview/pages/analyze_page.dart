import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/core/utils/datetime_formatter.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';
import 'package:mszczepienia/ui/submission_process/ocr_overview/pages/error_occured_page.dart';
import 'package:mszczepienia/ui/submission_process/ocr_overview/pages/loading_page.dart';

class AnalyzePage extends StatefulWidget {
  const AnalyzePage({super.key});

  @override
  State<AnalyzePage> createState() => _AnalyzePageState();
}

class _AnalyzePageState extends State<AnalyzePage> {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;

    return Layout(
      child: BlocBuilder<OcrSubmissionCubit, OcrSubmissionState>(
        builder: (context, state) {
          if (state is OcrSubmissionProcessingLoading) {
            return LoadingPage(message: l10n.analyzingSubmission);
          } else if (state is OcrSubmissionSubmittingLoading) {
            return LoadingPage(message: l10n.sendingSubmission);
          } else if (state is OcrSubmissionCancellingLoading) {
            return LoadingPage(message: l10n.cancelLoading);
          } else if (state is OcrSubmissionError) {
            return ErrorOccuredPage();
          } else if (state is OcrSubmissionLoaded) {
            return SingleChildScrollView(
              physics: const BouncingScrollPhysics(),
              child: Padding(
                padding: const EdgeInsets.only(
                  left: 24.0,
                  right: 24.0,
                  top: 8.0,
                  bottom: 20.0,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      l10n.vaccinationBooklet,
                      style: const TextStyle(
                        fontSize: 32,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                    const SizedBox(height: 20),
                    Padding(
                      padding: const EdgeInsets.only(right: 8.0),
                      child: Text(
                        l10n.ocrDescription,
                        style: const TextStyle(fontSize: 14),
                      ),
                    ),
                    const SizedBox(height: 20),
                    if (state.entries.isEmpty)
                      Center(child: Text(l10n.noVaccinations))
                    else
                      ListView.separated(
                        shrinkWrap: true,
                        physics: const NeverScrollableScrollPhysics(),
                        itemCount: state.entries.length,
                        separatorBuilder: (context, index) =>
                            const SizedBox(height: 12),
                        itemBuilder: (context, index) {
                          return Container(
                            padding: const EdgeInsets.all(12),
                            width: double.infinity,
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(16),
                              border: Border.all(
                                color: borderColor,
                                width: 1.1,
                              ),
                            ),
                            child: Row(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Expanded(
                                  child: Column(
                                    crossAxisAlignment:
                                        CrossAxisAlignment.start,
                                    children: [
                                      Text(
                                        state.entries[index].vaccineName,
                                        style: const TextStyle(
                                          fontSize: 16,
                                          fontWeight: FontWeight.bold,
                                        ),
                                      ),
                                      const SizedBox(height: 8),
                                      Text(
                                        state.entries[index].date ==
                                                DateTime(1900, 1, 1)
                                            ? '${l10n.vaccinationDate}: '
                                            : '${l10n.vaccinationDate}: ${DatetimeFormatter().datetimeFormatter(state.entries[index].date)}',
                                        style: const TextStyle(fontSize: 14),
                                      ),
                                      const SizedBox(height: 4),
                                      Text(
                                        '${l10n.batchNo}: ${state.entries[index].batch}',
                                        style: const TextStyle(fontSize: 14),
                                      ),
                                    ],
                                  ),
                                ),
                                Column(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    IconButton(
                                      icon: const Icon(
                                        Icons.delete_rounded,
                                        color: AppColors.logoRed,
                                      ),
                                      onPressed: () {
                                        showDialog(
                                          context: context,
                                          builder: (dialogContext) =>
                                              AlertDialog(
                                                title: Text(l10n.deleteEntry),
                                                content: Text(
                                                  l10n.deleteEntryDescription,
                                                ),
                                                actions: [
                                                  TextButton(
                                                    onPressed: () =>
                                                        Navigator.of(
                                                          dialogContext,
                                                        ).pop(),
                                                    child: Text(l10n.cancel),
                                                  ),
                                                  TextButton(
                                                    onPressed: () {
                                                      context
                                                          .read<
                                                            OcrSubmissionCubit
                                                          >()
                                                          .removeEntry(index);
                                                      Navigator.of(
                                                        dialogContext,
                                                      ).pop();
                                                    },
                                                    child: Text(
                                                      l10n.delete,
                                                      style: const TextStyle(
                                                        color:
                                                            AppColors.logoRed,
                                                      ),
                                                    ),
                                                  ),
                                                ],
                                              ),
                                        );
                                      },
                                    ),
                                    IconButton(
                                      icon: const Icon(Icons.edit_rounded),
                                      onPressed: () {
                                        context.push(
                                          '/submission-status/edit-entry?index=$index',
                                        );
                                      },
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          );
                        },
                      ),
                    const SizedBox(height: 32),
                    SizedBox(
                      width: double.infinity,
                      height: 56,
                      child: OutlinedButton(
                        onPressed: () {
                          context.push('/submission-status/add-entry');
                        },
                        style: OutlinedButton.styleFrom(
                          side: BorderSide(color: borderColor, width: 2.0),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(30),
                          ),
                        ),
                        child: Text(
                          l10n.addAnotherVaccination,
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),
                    SizedBox(
                      width: double.infinity,
                      height: 56,
                      child: OutlinedButton(
                        onPressed: () async {
                          await context
                              .read<OcrSubmissionCubit>()
                              .cancelOcrSubmission();
                          if (context.mounted) {
                            context.go('/');
                            context.push('/submission-status');
                          }
                        },
                        style: OutlinedButton.styleFrom(
                          side: BorderSide(color: borderColor, width: 2.0),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(30),
                          ),
                        ),
                        child: Text(
                          l10n.cancelSubmission,
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w600,
                            color: AppColors.logoRed,
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),
                    SizedBox(
                      width: double.infinity,
                      height: 56,
                      child: ElevatedButton(
                        onPressed: () async {
                          await context
                              .read<OcrSubmissionCubit>()
                              .submitOcrSubmission();
                          if (context.mounted) {
                            context.go('/');
                            context.push('/submission-status');
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
                          l10n.submitForVerification,
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w600,
                            color: Colors.white,
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            );
          }
          return const SizedBox.shrink();
        },
      ),
    );
  }
}
