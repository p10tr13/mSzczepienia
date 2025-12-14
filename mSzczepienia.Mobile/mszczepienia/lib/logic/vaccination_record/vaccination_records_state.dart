import 'package:mszczepienia/data/models/vaccination_record_overview_model.dart';

abstract class VaccinationRecordsState {}

class VaccinationRecordsInitial extends VaccinationRecordsState {}

class VaccinationRecordsLoading extends VaccinationRecordsState {}

class VaccinationRecordsLoaded extends VaccinationRecordsState {
  final List<VaccinationRecordOverviewModel> records;
  VaccinationRecordsLoaded(this.records);
}

class VaccinationRecordsError extends VaccinationRecordsState {
  final String message;
  VaccinationRecordsError(this.message);
}
