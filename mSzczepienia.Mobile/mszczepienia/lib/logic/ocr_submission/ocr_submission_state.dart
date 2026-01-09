import 'package:mszczepienia/data/models/vaccination_entry_model.dart';

abstract class OcrSubmissionState {}

class OcrSubmissionInitial extends OcrSubmissionState {}

class OcrSubmissionProcessingLoading extends OcrSubmissionState {}

class OcrSubmissionSubmittingLoading extends OcrSubmissionState {}

class OcrSubmissionCancellingLoading extends OcrSubmissionState {}

class OcrSubmissionLoaded extends OcrSubmissionState {
  final List<VaccinationEntryModel> entries;
  OcrSubmissionLoaded(this.entries);
}

class OcrSubmissionError extends OcrSubmissionState {
  final String message;
  OcrSubmissionError(this.message);
}
