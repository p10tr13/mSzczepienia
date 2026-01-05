import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';
import 'package:mszczepienia/ui/submission_process/entries_list.dart';

class AnalyzePage extends StatefulWidget {
  const AnalyzePage({super.key});

  @override
  State<AnalyzePage> createState() => _AnalyzePageState();
}

class _AnalyzePageState extends State<AnalyzePage> {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Layout(
      child: SingleChildScrollView(
        physics: BouncingScrollPhysics(),
        child: Padding(
          padding: const EdgeInsets.only(
            left: 24.0,
            right: 24.0,
            top: 8.0,
            bottom: 20.0,
          ),
          child: BlocBuilder<OcrSubmissionCubit, OcrSubmissionState>(
            builder: (context, state) {
              return Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    l10n.vaccinationBooklet,
                    style: TextStyle(fontSize: 32, fontWeight: FontWeight.w800),
                  ),
                  const SizedBox(height: 35),
                  if (state is OcrSubmissionLoading) ...[
                    Center(
                      child: Column(
                        children: [
                          const SizedBox(height: 80),
                          CircularProgressIndicator(),
                          const SizedBox(height: 16),
                          Text(
                            l10n.analyzingSubmission,
                            style: TextStyle(fontSize: 16),
                          ),
                        ],
                      ),
                    ),
                  ] else if (state is OcrSubmissionError) ...[
                    Column(
                      children: [
                        const SizedBox(height: 50),
                        Center(
                          child: Text(
                            l10n.errorOccurred,
                            style: TextStyle(
                              fontSize: 16,
                              color: AppColors.logoRed,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ] else if (state is OcrSubmissionLoaded) ...[
                    EntriesList(entries: state.entries),
                  ],
                ],
              );
            },
          ),
        ),
      ),
    );
  }
}
