import 'package:mszczepienia/data/models/vaccination_record_details_model.dart';

abstract class VaccinationRecordDetailsState {}

class VaccinationRecordDetailsInitial extends VaccinationRecordDetailsState {}

class VaccinationRecordDetailsLoading extends VaccinationRecordDetailsState {}

class VaccinationRecordDetailsLoaded extends VaccinationRecordDetailsState {
  final List<VaccinationRecordDetailsModel> details;
  VaccinationRecordDetailsLoaded(this.details);
}

class VaccinationRecordDetailsError extends VaccinationRecordDetailsState {
  final String message;
  VaccinationRecordDetailsError(this.message);
}
