import 'package:dio/dio.dart';
import 'package:mszczepienia/data/models/medical_facility_model.dart';

class MedicalFacilityRemoteDatasource {
  final Dio dio;

  MedicalFacilityRemoteDatasource(this.dio);

  Future<List<MedicalFacilityModel>> fetchMedicalFacilities() async {
    final response = await dio.get('/facilities/facility-names');

    final data = response.data as List;

    return data.map((json) => MedicalFacilityModel.fromJson(json)).toList();
  }
}
