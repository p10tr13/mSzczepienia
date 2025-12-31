import 'package:mszczepienia/data/models/citizen_submission_status.dart';

abstract class CitizenState {}

class CitizenInitial extends CitizenState {}

class CitizenLoading extends CitizenState {}

class CitizenLoaded extends CitizenState {
  final CitizenSubmissionStatus status;
  CitizenLoaded(this.status);
}

class CitizenError extends CitizenState {
  final String message;
  CitizenError(this.message);
}
