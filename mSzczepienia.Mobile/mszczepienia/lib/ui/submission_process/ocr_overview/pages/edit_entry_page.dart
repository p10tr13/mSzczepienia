import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';
import 'package:mszczepienia/ui/authorization/custom_text_field.dart';
import 'package:mszczepienia/ui/layout/layout.dart';

class EditEntryPage extends StatefulWidget {
  const EditEntryPage({super.key, required this.index});

  final int index;

  @override
  State<EditEntryPage> createState() => _EditEntryPageState();
}

class _EditEntryPageState extends State<EditEntryPage> {
  final _formKey = GlobalKey<FormState>();

  late final TextEditingController _dateController;
  late final TextEditingController _batchController;
  late final VaccinationEntryModel _originalEntry;

  @override
  void initState() {
    super.initState();
    final state = context.read<OcrSubmissionCubit>().state;
    if (state is OcrSubmissionLoaded) {
      _originalEntry = state.entries[widget.index];
      _dateController = TextEditingController(
        text: _originalEntry.date == DateTime(1900, 1, 1)
            ? ''
            : DateFormat('yyyy-MM-dd').format(_originalEntry.date),
      );
      _batchController = TextEditingController(text: _originalEntry.batch);
    }
  }

  @override
  void dispose() {
    _dateController.dispose();
    _batchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;
    final grey = Theme.of(context).colorScheme.onSurfaceVariant;

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
                    l10n.modifyEntry,
                    style: const TextStyle(
                      fontSize: 32,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    _originalEntry.vaccineName,
                    style: TextStyle(
                      fontSize: 16,
                      color: grey,
                      fontWeight: FontWeight.w500,
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
                            CustomTextField(
                              label: l10n.vaccinationDate,
                              controller: _dateController,
                              showEyeIcon: false,
                              keyboardType: TextInputType.datetime,
                              hintText: 'YYYY-MM-DD',
                              readOnly: true,
                              onTap: () async {
                                final DateTime? picked = await showDatePicker(
                                  context: context,
                                  initialDate: _originalEntry.date == DateTime(1900, 1, 1)
                                      ? DateTime.now()
                                      : _originalEntry.date,
                                  firstDate: DateTime(1900),
                                  lastDate: DateTime.now(),
                                );
                                if (picked != null) {
                                  _dateController.text =
                                      '${picked.year.toString().padLeft(4, '0')}-${picked.month.toString().padLeft(2, '0')}-${picked.day.toString().padLeft(2, '0')}';
                                }
                              },
                              validator: (value) {
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
                            if (_formKey.currentState!.validate()) {
                              final updatedEntry = VaccinationEntryModel(
                                id: _originalEntry.id,
                                vaccineNameOcr: _originalEntry.vaccineNameOcr,
                                dateOcr: _originalEntry.dateOcr,
                                batchOcr: _originalEntry.batchOcr,
                                vaccineName: _originalEntry.vaccineName,
                                date: _dateController.text.isNotEmpty
                                    ? DateTime.parse(_dateController.text)
                                    : DateTime(1900, 1, 1),
                                batch: _batchController.text.toUpperCase(),
                                status: _originalEntry.status,
                              );
                              context.read<OcrSubmissionCubit>().updateEntry(
                                widget.index,
                                updatedEntry,
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
