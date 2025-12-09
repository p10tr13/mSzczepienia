import 'package:mszczepienia/data/models/medical_facility_model.dart';

abstract class FacilitiesState {}

class FacilitiesInitial extends FacilitiesState {}

class FacilitiesLoading extends FacilitiesState {}

class FacilitiesLoaded extends FacilitiesState {
  final List<MedicalFacilityModel> facilities;
  FacilitiesLoaded(this.facilities);
}

class FacilitiesError extends FacilitiesState {
  final String message;
  FacilitiesError(this.message);
}
