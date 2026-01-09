import 'package:dio/dio.dart';
import 'package:mszczepienia/data/datasources/vaccine_remote_datasource.dart';
import 'package:mszczepienia/data/models/vaccine_name_model.dart';

class VaccineRepository {
  final VaccineRemoteDatasource remoteDataSource;

  VaccineRepository(this.remoteDataSource);

  Future<List<VaccineNameModel>> getVaccineNames(String query) async {
    try {
      return await remoteDataSource.fetchVaccineNames(query);
    } on DioException catch (e) {
      throw Exception(e.response?.data ?? "Connection Error");
    } catch (e) {
      throw Exception("Unknown Error: $e");
    }
  }
}
