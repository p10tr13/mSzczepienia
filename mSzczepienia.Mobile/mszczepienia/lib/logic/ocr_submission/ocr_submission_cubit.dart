import 'dart:io';

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/data/models/ocr_submission_model.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';
import 'package:mszczepienia/data/repositories/ocr_submission_repository.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';

class OcrSubmissionCubit extends Cubit<OcrSubmissionState> {
  final OcrSubmissionRepository repository;

  OcrSubmissionCubit(this.repository) : super(OcrSubmissionInitial());

  Future<void> processVaccinationBooklet(List<File> images) async {
    emit(OcrSubmissionProcessingLoading());

    OcrSubmissionModel query = OcrSubmissionModel(images: images);

    try {
      final entries = await repository.processVaccinationBooklet(query);
      emit(OcrSubmissionLoaded(entries));
    } catch (e) {
      emit(OcrSubmissionError(e.toString()));
    }
  }

  Future<void> cancelOcrSubmission() async {
    emit(OcrSubmissionCancellingLoading());
    try {
      await repository.cancelOcrSubmission();
      emit(OcrSubmissionInitial());
    } catch (e) {
      emit(OcrSubmissionError(e.toString()));
    }
  }

  Future<void> submitOcrSubmission() async {
    if (state is OcrSubmissionLoaded) {
      final currentState = state as OcrSubmissionLoaded;
      final entries = List<VaccinationEntryModel>.from(currentState.entries);
      emit(OcrSubmissionSubmittingLoading());
      try {
        await repository.submitOcrSubmission(entries);
        emit(OcrSubmissionInitial());
      } catch (e) {
        emit(OcrSubmissionError(e.toString()));
      }
    }
  }

  void addEntry(VaccinationEntryModel entry) {
    if (state is OcrSubmissionLoaded) {
      final currentState = state as OcrSubmissionLoaded;
      final newEntries = List<VaccinationEntryModel>.from(currentState.entries);
      newEntries.add(entry);
      emit(OcrSubmissionLoaded(newEntries));
    }
  }

  void updateEntry(int index, VaccinationEntryModel entry) {
    if (state is OcrSubmissionLoaded) {
      final currentState = state as OcrSubmissionLoaded;
      final newEntries = List<VaccinationEntryModel>.from(currentState.entries);
      newEntries[index] = entry;
      emit(OcrSubmissionLoaded(newEntries));
    }
  }

  void removeEntry(int index) {
    if (state is OcrSubmissionLoaded) {
      final currentState = state as OcrSubmissionLoaded;
      final newEntries = List<VaccinationEntryModel>.from(currentState.entries);
      newEntries.removeAt(index);
      emit(OcrSubmissionLoaded(newEntries));
    }
  }
}
