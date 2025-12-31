import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/core/utils/datetime_formatter.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/logic/language/language_state.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_cubit.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';
import 'package:mszczepienia/ui/vaccination_details/vaccination_dose_container.dart';

class VaccinationDetailsPage extends StatefulWidget {
  const VaccinationDetailsPage({super.key, required this.vaccineId});

  final int vaccineId;

  @override
  State<VaccinationDetailsPage> createState() => _VaccinationDetailsPageState();
}

class _VaccinationDetailsPageState extends State<VaccinationDetailsPage> {
  @override
  void initState() {
    super.initState();

    context.read<VaccinationRecordDetailsCubit>().loadVaccinationDetails(
      widget.vaccineId,
    );
    setState(() {});
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
            bottom: 20.0,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                l10n.yourVaccinations,
                style: const TextStyle(
                  fontSize: 32,
                  fontWeight: FontWeight.w800,
                ),
              ),
              const SizedBox(height: 20),
              Container(
                padding: const EdgeInsets.all(20),
                width: double.infinity,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: borderColor, width: 1.1),
                ),
                child:
                    BlocBuilder<
                      VaccinationRecordDetailsCubit,
                      VaccinationRecordDetailsState
                    >(
                      builder: (context, state) {
                        if (state is VaccinationRecordDetailsInitial ||
                            state is VaccinationRecordDetailsLoading) {
                          return const Center(
                            child: CircularProgressIndicator(),
                          );
                        } else if (state is VaccinationRecordDetailsError) {
                          return Center(
                            child: Text(l10n.errorLoadingVaccinationDetails),
                          );
                        } else if (state is VaccinationRecordDetailsLoaded) {
                          final details = state.details;
                          return Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  ElevatedButton(
                                    onPressed: () {},
                                    style: ElevatedButton.styleFrom(
                                      shape: CircleBorder(),
                                      padding: EdgeInsets.all(7.0),
                                      backgroundColor: AppColors.lightGrey,
                                      foregroundColor: AppColors.lightGrey,
                                    ),
                                    child: Icon(
                                      Icons.vaccines_rounded,
                                      color: Colors.black,
                                      size: 20.0,
                                    ),
                                  ),
                                  const SizedBox(width: 15),
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment:
                                          CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          details.first.vaccineName,
                                          style: TextStyle(
                                            fontSize: 15,
                                            fontWeight: FontWeight.bold,
                                          ),
                                        ),
                                        const SizedBox(height: 5),
                                        BlocBuilder<
                                          LanguageCubit,
                                          LanguageState
                                        >(
                                          builder: (context, state) {
                                            return Text(
                                              state is LanguageEnglish
                                                  ? details
                                                        .first
                                                        .shortDescriptionEn
                                                  : details
                                                        .first
                                                        .shortDescriptionPl,
                                              style: TextStyle(fontSize: 14),
                                            );
                                          },
                                        ),
                                      ],
                                    ),
                                  ),
                                ],
                              ),
                              const SizedBox(height: 30),
                              ListView.separated(
                                shrinkWrap: true,
                                physics: NeverScrollableScrollPhysics(),
                                separatorBuilder: (context, index) => Container(
                                  margin: const EdgeInsets.symmetric(
                                    vertical: 10,
                                  ),
                                  height: 1,
                                  color: borderColor,
                                ),
                                itemBuilder: (context, index) {
                                  final record = details[index];
                                  return VaccinationDoseContainer(
                                    doseNumber: record.doseNumber,
                                    vaccinationDate: record.vaccinationDate,
                                    batchNumber: record.batch,
                                    remarks: record.remarks,
                                    nextVaccinationDate:
                                        record.nextVaccinationDate,
                                  );
                                },
                                itemCount: details.length,
                              ),
                            ],
                          );
                        }
                        return Center(
                          child: Text(l10n.errorLoadingVaccinationDetails),
                        );
                      },
                    ),
              ),
              const SizedBox(height: 20),
              Container(
                padding: const EdgeInsets.all(20),
                width: double.infinity,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: borderColor, width: 1.1),
                ),
                child:
                    BlocBuilder<
                      VaccinationRecordDetailsCubit,
                      VaccinationRecordDetailsState
                    >(
                      builder: (context, state) {
                        if (state is VaccinationRecordDetailsInitial ||
                            state is VaccinationRecordDetailsLoading) {
                          return const Center(
                            child: CircularProgressIndicator(),
                          );
                        } else if (state is VaccinationRecordDetailsError) {
                          return Center(
                            child: Text(l10n.errorLoadingVaccinationDetails),
                          );
                        } else if (state is VaccinationRecordDetailsLoaded) {
                          final details = state.details;
                          return Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                l10n.aboutVaccine,
                                style: const TextStyle(
                                  fontSize: 20,
                                  fontWeight: FontWeight.w700,
                                  color: AppColors.logoRed,
                                ),
                              ),
                              const SizedBox(height: 15),
                              Text(
                                details.first.vaccineName,
                                style: const TextStyle(
                                  fontSize: 18,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                l10n.vaccineDescription,
                                style: const TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                              const SizedBox(height: 5),
                              BlocBuilder<LanguageCubit, LanguageState>(
                                builder: (context, state) {
                                  return Text(
                                    state is LanguagePolish
                                        ? details.first.vaccineDescriptionPl
                                        : details.first.vaccineDescriptionEn,
                                    style: const TextStyle(fontSize: 14),
                                  );
                                },
                              ),
                              const SizedBox(height: 10),
                              Text(
                                l10n.diseasesPrevented,
                                style: const TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                              const SizedBox(height: 5),
                              BlocBuilder<LanguageCubit, LanguageState>(
                                builder: (context, state) {
                                  final diseases = state is LanguagePolish
                                      ? details.first.diseasesPl.join(', ')
                                      : details.first.diseasesEn.join(', ');
                                  return Text(
                                    diseases,
                                    style: const TextStyle(fontSize: 14),
                                  );
                                },
                              ),
                              const SizedBox(height: 10),
                              Text(
                                l10n.producer,
                                style: const TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                              const SizedBox(height: 5),
                              Text(
                                details.first.manufacturer,
                                style: const TextStyle(fontSize: 14),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                l10n.vaccineCreationDate,
                                style: const TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                              const SizedBox(height: 5),
                              Text(
                                DatetimeFormatter().datetimeFormatter(
                                  details.first.creationDate,
                                ),
                                style: const TextStyle(fontSize: 14),
                              ),
                            ],
                          );
                        }
                        return Center(
                          child: Text(l10n.errorLoadingVaccinationDetails),
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
