import 'package:mszczepienia/data/models/vaccine_name_model.dart';

abstract class VaccinesState {}

class VaccinesInitial extends VaccinesState {}

class VaccinesLoading extends VaccinesState {}

class VaccinesLoaded extends VaccinesState {
  final List<VaccineNameModel> vaccines;
  VaccinesLoaded(this.vaccines);
}

class VaccinesError extends VaccinesState {
  final String message;
  VaccinesError(this.message);
}
