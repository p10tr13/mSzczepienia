import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/data/queries/vaccination_record_details_query.dart';
import 'package:mszczepienia/data/repositories/vaccination_record_repository.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_state.dart';

class VaccinationRecordDetailsCubit
    extends Cubit<VaccinationRecordDetailsState> {
  final VaccinationRecordRepository repository;

  VaccinationRecordDetailsCubit(this.repository)
    : super(VaccinationRecordDetailsInitial());

  Future<void> loadVaccinationDetails(int vaccineId) async {
    emit(VaccinationRecordDetailsLoading());

    VaccinationRecordDetailsQuery query = VaccinationRecordDetailsQuery(
      vaccineId: vaccineId,
    );
    try {
      final details = await repository.getVaccinationRecordsByVaccineId(query);
      emit(VaccinationRecordDetailsLoaded(details));
    } catch (e) {
      emit(VaccinationRecordDetailsError(e.toString()));
    }
  }
}
