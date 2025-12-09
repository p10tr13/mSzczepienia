import 'package:mszczepienia/data/models/citizen_details_model.dart';

abstract class CitizenState {}

class CitizenInitial extends CitizenState {}

class CitizenLoading extends CitizenState {}

class CitizenLoaded extends CitizenState {
  final CitizenDetailsModel details;
  CitizenLoaded(this.details);
}

class CitizenError extends CitizenState {
  final String message;
  CitizenError(this.message);
}
