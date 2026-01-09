import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';
import 'package:mszczepienia/ui/authorization/custom_text_field.dart';
import 'package:mszczepienia/ui/layout/layout.dart';
import 'package:mszczepienia/ui/submission_process/ocr_overview/vaccine_name_field.dart';

class AddEntryPage extends StatefulWidget {
  const AddEntryPage({super.key});

  @override
  State<AddEntryPage> createState() => _AddEntryPageState();
}

class _AddEntryPageState extends State<AddEntryPage> {
  final _formKey = GlobalKey<FormState>();

  final TextEditingController _vaccineNameController = TextEditingController();
  final TextEditingController _dateController = TextEditingController();
  final TextEditingController _batchController = TextEditingController();

  @override
  void dispose() {
    _vaccineNameController.dispose();
    _dateController.dispose();
    _batchController.dispose();
    super.dispose();
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
            bottom: 20.0,
          ),
          child: Column(
            children: [
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    l10n.addAnotherVaccination,
                    style: const TextStyle(
                      fontSize: 32,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  const SizedBox(height: 30),
                  Container(
                    padding: const EdgeInsets.all(20),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(16),
                      border: Border.all(color: borderColor, width: 1.1),
                    ),
                    child: Form(
                      key: _formKey,
                      child: Padding(
                        padding: const EdgeInsets.all(8.0),
                        child: Column(
                          children: [
                            VaccineNameField(
                              controller: _vaccineNameController,
                            ),
                            const SizedBox(height: 16),
                            CustomTextField(
                              label: l10n.vaccinationDate,
                              controller: _dateController,
                              showEyeIcon: false,
                              keyboardType: TextInputType.datetime,
                              hintText: 'YYYY-MM-DD',
                              validator: (value) {
                                if (value == null || value.isEmpty) {
                                  return null;
                                }
                                final dateRegex = RegExp(
                                  r'^\d{4}-\d{2}-\d{2}$',
                                );
                                if (!dateRegex.hasMatch(value)) {
                                  return l10n.dateOfVacccinationCondition;
                                }
                                try {
                                  DateTime.parse(value);
                                } catch (e) {
                                  return l10n.dateOfVacccinationCondition;
                                }
                                return null;
                              },
                            ),
                            const SizedBox(height: 16),
                            CustomTextField(
                              label: l10n.batchNo,
                              controller: _batchController,
                              showEyeIcon: false,
                              keyboardType: TextInputType.text,
                              validator: (value) {
                                if (value != null && value.length > 20) {
                                  return l10n.batchCondition;
                                }
                                return null;
                              },
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 24),
                  SizedBox(
                    width: double.infinity,
                    height: 56,
                    child: BlocBuilder<OcrSubmissionCubit, OcrSubmissionState>(
                      builder: (context, state) {
                        return ElevatedButton(
                          onPressed: () {
                            if (_formKey.currentState!.validate() &&
                                state is OcrSubmissionLoaded) {
                              final newEntry = VaccinationEntryModel(
                                id: state.entries.length,
                                vaccineNameOcr: _vaccineNameController.text,
                                dateOcr: _dateController.text,
                                batchOcr: _batchController.text,
                                vaccineName: _vaccineNameController.text,
                                date: _dateController.text.isNotEmpty
                                    ? DateTime.parse(_dateController.text)
                                    : DateTime(1900, 1, 1),
                                batch: _batchController.text.toUpperCase(),
                                status: 'CreatedByUser',
                              );
                              context.read<OcrSubmissionCubit>().addEntry(
                                newEntry,
                              );
                              context.pop();
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
                            l10n.saveChanges,
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
