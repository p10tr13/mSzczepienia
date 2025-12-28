import 'dart:io';

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/data/models/ocr_submission_model.dart';
import 'package:mszczepienia/data/repositories/ocr_submission_repository.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';

class OcrSubmissionCubit extends Cubit<OcrSubmissionState> {
  final OcrSubmissionRepository repository;

  OcrSubmissionCubit(this.repository) : super(OcrSubmissionInitial());

  Future<void> processVaccinationBooklet(List<File> images) async {
    emit(OcrSubmissionLoading());

    OcrSubmissionModel query = OcrSubmissionModel(images: images);

    try {
      final entries = await repository.processVaccinationBooklet(query);
      emit(OcrSubmissionLoaded(entries));
    } catch (e) {
      emit(OcrSubmissionError(e.toString()));
    }
  }
}
