import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/data/queries/vaccination_record_overview_query.dart';
import 'package:mszczepienia/data/repositories/vaccination_record_repository.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_state.dart';

class VaccinationRecordsCubit extends Cubit<VaccinationRecordsState> {
  final VaccinationRecordRepository repository;

  VaccinationRecordsCubit(this.repository) : super(VaccinationRecordsInitial());

  Future<void> loadHistory(
    int pageNumber,
    int pageSize,
    String sortingField,
    String sortingDirection,
  ) async {
    emit(VaccinationRecordsLoading());

    VaccinationRecordOverviewQuery query = VaccinationRecordOverviewQuery(
      pageNumber: pageNumber,
      pageSize: pageSize,
      sortingField: sortingField,
      sortingDirection: sortingDirection,
    );

    try {
      final records = await repository.getVaccinationHistory(query);
      emit(VaccinationRecordsLoaded(records));
    } catch (e) {
      emit(VaccinationRecordsError(e.toString()));
    }
  }
}
