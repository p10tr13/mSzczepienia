import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/data/models/medical_facility_model.dart';
import 'package:mszczepienia/logic/facility/facilities_cubit.dart';
import 'package:mszczepienia/logic/facility/facilities_state.dart';

class MedicalFacilityField extends StatefulWidget {
  const MedicalFacilityField({super.key, required this.controller});

  final TextEditingController controller;

  @override
  State<MedicalFacilityField> createState() => _MedicalFacilityFieldState();
}

class _MedicalFacilityFieldState extends State<MedicalFacilityField> {
  late List<MedicalFacilityModel> facilities = [];
  bool facilitiesLoaded = false;

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<FacilitiesCubit, FacilitiesState>(
      builder: (context, facilitiesState) {
        // Load facilities on first build if not loaded yet
        if (facilitiesState is FacilitiesInitial) {
          context.read<FacilitiesCubit>().loadFacilities();
        }

        // Update facilities when loaded
        if (facilitiesState is FacilitiesLoaded) {
          facilities = facilitiesState.facilities;
          facilitiesLoaded = true;
        }

        // Show error state if failed
        if (facilitiesState is FacilitiesError) {
          return _buildErrorField(facilitiesState.message);
        }

        // Always show autocomplete field, even while loading
        return _buildAutocompleteField(facilities);
      },
    );
  }

  Widget _buildAutocompleteField(List<MedicalFacilityModel> facilities) {
    return Autocomplete<String>(
      optionsBuilder: (TextEditingValue textEditingValue) {
        if (textEditingValue.text.isEmpty) {
          return facilities.map((f) => f.facilityName).toList();
        }
        return facilities
            .where(
              (facility) => facility.facilityName.toLowerCase().contains(
                textEditingValue.text.toLowerCase(),
              ),
            )
            .map((facility) => facility.facilityName)
            .toList();
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
              },
              decoration: InputDecoration(
                labelText: 'Default Medical Facility',
                labelStyle: const TextStyle(
                  color: AppColors.naturalGrey,
                  fontWeight: FontWeight.w500,
                ),
                filled: true,
                fillColor: AppColors.textFieldFill,
                contentPadding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 16,
                ),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                  borderSide: const BorderSide(
                    color: AppColors.textFieldBorder,
                  ),
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                  borderSide: const BorderSide(
                    color: AppColors.textFieldBorder,
                  ),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                  borderSide: const BorderSide(
                    color: AppColors.buttonBlue,
                    width: 2,
                  ),
                ),
              ),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Please select a medical facility';
                }
                if (!facilities.any(
                  (facility) => facility.facilityName == value,
                )) {
                  return 'Please select a valid medical facility from the list';
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
  }

  Widget _buildErrorField(String errorMessage) {
    return TextField(
      enabled: false,
      decoration: InputDecoration(
        labelText: errorMessage,
        labelStyle: const TextStyle(
          color: Colors.red,
          fontWeight: FontWeight.w500,
        ),
        filled: true,
        fillColor: AppColors.textFieldFill,
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
}
