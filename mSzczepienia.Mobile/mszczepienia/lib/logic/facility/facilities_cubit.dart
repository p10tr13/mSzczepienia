import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/data/repositories/medical_facility_repository.dart';
import 'package:mszczepienia/logic/facility/facilities_state.dart';

class FacilitiesCubit extends Cubit<FacilitiesState> {
  final MedicalFacilityRepository repository;

  FacilitiesCubit(this.repository) : super(FacilitiesInitial());

  Future<void> loadFacilities(String query) async {
    emit(FacilitiesLoading());
    try {
      final details = await repository.getMedicalFacilities(query);
      emit(FacilitiesLoaded(details));
    } catch (e) {
      emit(FacilitiesError(e.toString()));
    }
  }
}
