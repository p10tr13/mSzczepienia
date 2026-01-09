import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/data/repositories/vaccine_repository.dart';
import 'package:mszczepienia/logic/vaccine/vaccines_state.dart';

class VaccinesCubit extends Cubit<VaccinesState> {
  final VaccineRepository repository;

  VaccinesCubit(this.repository) : super(VaccinesInitial());

  Future<void> loadVaccines(String query) async {
    emit(VaccinesLoading());
    try {
      final vaccines = await repository.getVaccineNames(query);
      emit(VaccinesLoaded(vaccines));
    } catch (e) {
      emit(VaccinesError(e.toString()));
    }
  }
}
