import 'package:dio/dio.dart';
import 'package:mszczepienia/data/datasources/citizen_remote_datasource.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';

class CitizenRepository {
  final CitizenRemoteDataSource remoteDataSource;

  CitizenRepository(this.remoteDataSource);

  Future<CitizenDetailsModel> getCitizenDetails() async {
    try {
      return await remoteDataSource.fetchCitizenDetails();
    } on DioException catch (e) {
      throw Exception(e.response?.data ?? "Connection Error");
    } catch (e) {
      throw Exception("Unknown Error");
    }
  }
}