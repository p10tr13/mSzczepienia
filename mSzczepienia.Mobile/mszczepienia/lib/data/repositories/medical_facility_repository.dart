import 'package:dio/dio.dart';
import 'package:mszczepienia/data/datasources/medical_facility_remote_datasource.dart';
import 'package:mszczepienia/data/models/medical_facility_model.dart';

class MedicalFacilityRepository {
  final MedicalFacilityRemoteDatasource remoteDataSource;

  MedicalFacilityRepository(this.remoteDataSource);

  Future<List<MedicalFacilityModel>> getMedicalFacilities() async {
    try {
      return await remoteDataSource.fetchMedicalFacilities();
    } on DioException catch (e) {
      throw Exception(e.response?.data ?? "Connection Error");
    } catch (e) {
      throw Exception("Unknown Error");
    }
  }
}
