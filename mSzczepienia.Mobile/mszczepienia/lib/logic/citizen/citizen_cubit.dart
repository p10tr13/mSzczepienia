import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';
import 'package:mszczepienia/logic/citizen/citizen_state.dart';

class CitizenCubit extends Cubit<CitizenState> {
  final CitizenRepository repository;

  CitizenCubit(this.repository) : super(CitizenInitial());

  Future<void> loadCitizenDetails() async {
    emit(CitizenLoading());
    try {
      final details = await repository.getCitizenDetails();
      emit(CitizenLoaded(details));
    } catch (e) {
      emit(CitizenError(e.toString()));
    }
  }
}