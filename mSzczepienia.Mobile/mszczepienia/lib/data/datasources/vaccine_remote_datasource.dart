import 'package:dio/dio.dart';
import 'package:mszczepienia/data/models/vaccine_name_model.dart';

class VaccineRemoteDatasource {
  final Dio dio;

  VaccineRemoteDatasource(this.dio);

  Future<List<VaccineNameModel>> fetchVaccineNames(String query) async {
    final response = await dio.get(
      '/vaccines/vaccine-names',
      queryParameters: {'query': query},
    );

    final data = response.data as List;

    return data.map((json) => VaccineNameModel.fromJson(json)).toList();
  }
}
