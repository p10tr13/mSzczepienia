import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/data/models/vaccine_name_model.dart';
import 'package:mszczepienia/logic/vaccine/vaccines_cubit.dart';
import 'package:mszczepienia/logic/vaccine/vaccines_state.dart';

class VaccineNameField extends StatefulWidget {
  const VaccineNameField({super.key, required this.controller});

  final TextEditingController controller;

  @override
  State<VaccineNameField> createState() => _VaccineNameFieldState();
}

class _VaccineNameFieldState extends State<VaccineNameField> {
  List<VaccineNameModel> vaccines = [];
  Timer? _debounce;

  @override
  void dispose() {
    _debounce?.cancel();
    super.dispose();
  }

  void _onSearchChanged(String query) {
    if (_debounce?.isActive ?? false) _debounce!.cancel();
    _debounce = Timer(const Duration(milliseconds: 300), () {
      if (query.isNotEmpty) {
        context.read<VaccinesCubit>().loadVaccines(query);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<VaccinesCubit, VaccinesState>(
      builder: (context, vaccinesState) {
        if (vaccinesState is VaccinesLoaded) {
          vaccines = vaccinesState.vaccines;
        }

        if (vaccinesState is VaccinesError) {
          return TextField(
            enabled: false,
            decoration: InputDecoration(
              labelText: vaccinesState.message,
              labelStyle: const TextStyle(
                color: Colors.red,
                fontWeight: FontWeight.w500,
              ),
              filled: true,
              fillColor: AppColors.lightGrey,
              contentPadding: const EdgeInsets.symmetric(
                horizontal: 16,
                vertical: 16,
              ),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(8),
                borderSide: const BorderSide(color: Colors.red),
              ),
              enabledBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(8),
                borderSide: const BorderSide(color: Colors.red),
              ),
              disabledBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(8),
                borderSide: const BorderSide(color: Colors.red),
              ),
            ),
          );
        }

        return Autocomplete<String>(
          optionsBuilder: (TextEditingValue textEditingValue) {
            return vaccines.map((v) => v.vaccineName).toList();
          },
          onSelected: (String selection) {
            widget.controller.text = selection;
          },
          fieldViewBuilder:
              (
                BuildContext context,
                TextEditingController textEditingController,
                FocusNode focusNode,
                VoidCallback onFieldSubmitted,
              ) {
                widget.controller.addListener(() {
                  textEditingController.text = widget.controller.text;
                });
                return TextFormField(
                  controller: textEditingController,
                  focusNode: focusNode,
                  onChanged: (value) {
                    widget.controller.text = value;
                    _onSearchChanged(value);
                  },
                  style: TextStyle(color: AppColors.darkGrey),
                  decoration: InputDecoration(
                    labelText: l10n.vaccineName,
                    labelStyle: const TextStyle(
                      color: AppColors.naturalGrey,
                      fontWeight: FontWeight.w500,
                    ),
                    filled: true,
                    fillColor: AppColors.lightGrey,
                    contentPadding: const EdgeInsets.symmetric(
                      horizontal: 16,
                      vertical: 16,
                    ),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide: const BorderSide(
                        color: AppColors.naturalGrey,
                      ),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide: const BorderSide(
                        color: AppColors.naturalGrey,
                      ),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide: const BorderSide(
                        color: AppColors.defaultBlue,
                        width: 2,
                      ),
                    ),
                    errorBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide: const BorderSide(color: Colors.red, width: 1),
                    ),
                    focusedErrorBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(8),
                      borderSide: const BorderSide(color: Colors.red, width: 2),
                    ),
                  ),
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return l10n.vaccineNameCondition;
                    }
                    if (!vaccines.any(
                      (vaccine) => vaccine.vaccineName == value,
                    )) {
                      return l10n.vaccineNameCondition;
                    }
                    return null;
                  },
                );
              },
          optionsViewBuilder:
              (
                BuildContext context,
                AutocompleteOnSelected<String> onSelected,
                Iterable<String> options,
              ) {
                return Align(
                  alignment: Alignment.topLeft,
                  child: Material(
                    elevation: 4.0,
                    borderRadius: BorderRadius.circular(8),
                    child: Container(
                      constraints: const BoxConstraints(maxHeight: 200),
                      child: ListView.builder(
                        padding: const EdgeInsets.symmetric(vertical: 4),
                        itemCount: options.length,
                        itemBuilder: (BuildContext context, int index) {
                          final String option = options.elementAt(index);
                          return InkWell(
                            onTap: () {
                              onSelected(option);
                            },
                            child: Padding(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 16.0,
                                vertical: 10.0,
                              ),
                              child: Text(option),
                            ),
                          );
                        },
                      ),
                    ),
                  ),
                );
              },
        );
      },
    );
  }
}
