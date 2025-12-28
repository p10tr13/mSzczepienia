import 'package:dio/dio.dart';
import 'package:mszczepienia/data/datasources/ocr_submission_remote_datasource.dart';
import 'package:mszczepienia/data/models/ocr_submission_model.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';

class OcrSubmissionRepository {
  final OcrSubmissionRemoteDatasource remoteDataSource;

  OcrSubmissionRepository(this.remoteDataSource);

  Future<List<VaccinationEntryModel>> processVaccinationBooklet(
    OcrSubmissionModel query,
  ) async {
    try {
      return await remoteDataSource.processVaccinationBooklet(query);
    } on DioException catch (e) {
      throw Exception(e.response?.data ?? "Connection Error");
    } catch (e) {
      throw Exception("Unknown Error");
    }
  }
}
