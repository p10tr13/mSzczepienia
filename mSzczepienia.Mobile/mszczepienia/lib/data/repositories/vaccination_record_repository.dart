import 'package:dio/dio.dart';
import 'package:mszczepienia/data/datasources/vaccination_record_remote_datasource.dart';
import 'package:mszczepienia/data/models/vaccination_record_details_model.dart';
import 'package:mszczepienia/data/models/vaccination_record_overview_model.dart';
import 'package:mszczepienia/data/queries/vaccination_record_details_query.dart';
import 'package:mszczepienia/data/queries/vaccination_record_overview_query.dart';

class VaccinationRecordRepository {
  final VaccinationRecordRemoteDatasource remoteDataSource;

  VaccinationRecordRepository(this.remoteDataSource);

  Future<List<VaccinationRecordOverviewModel>> getVaccinationHistory(
    VaccinationRecordOverviewQuery query,
  ) async {
    try {
      return await remoteDataSource.fetchVaccinationHistory(query);
    } on DioException catch (e) {
      throw Exception(e.response?.data ?? "Connection Error");
    } catch (e) {
      throw Exception("Unknown Error");
    }
  }

  Future<List<VaccinationRecordDetailsModel>> getVaccinationRecordsByVaccineId(
    VaccinationRecordDetailsQuery query,
  ) async {
    try {
      return await remoteDataSource.fetchVaccinationRecordsByVaccineId(query);
    } on DioException catch (e) {
      throw Exception(e.response?.data ?? "Connection Error");
    } catch (e) {
      throw Exception("Unknown Error");
    }
  }
}
